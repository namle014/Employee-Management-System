using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OA.Core.Configurations;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Globalization;
using System.Linq.Expressions;
using System.Web;

namespace OA.Service
{
    public class SysFileService : BaseService<SysFile, SysFileCreateVModel, SysFileUpdateVModel, SysFileGetByIdVModel, SysFileGetAllVModel, SysFileExportVModel>, ISysFileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseRepository<SysFile> _sysFileRepo;
        private readonly JwtIssuerOptions _jwtIssuerOptions;
        private readonly IMapper _mapper;
        private readonly string _tempFolder;
        private readonly string _tempPath;
        private readonly ILogger _logger;
        private readonly int _chunkSize;
        private readonly string[] _medias = { CommonConstants.FileType.Audio, CommonConstants.FileType.Image, CommonConstants.FileType.Video, CommonConstants.FileType.Document };
        private readonly IWebHostEnvironment _env;
        private readonly UploadConfigurations _uploadConfigs;

        public SysFileService(IBaseRepository<SysFile> sysFileRepo,
            IMapper mapper,
            ILogger<SysFileService> logger,
            IHttpContextAccessor contextAccessor,
            IOptions<UploadConfigurations> uploadConfigs,
            IWebHostEnvironment env,
            IOptions<JwtIssuerOptions> jwtIssuerOptions,
            IHttpContextAccessor httpContextAccessor) : base(sysFileRepo, mapper)
        {
            _logger = logger;
            _sysFileRepo = sysFileRepo;
            _mapper = mapper;
            _jwtIssuerOptions = jwtIssuerOptions.Value;
            _uploadConfigs = uploadConfigs.Value;
            _env = env;
            _tempFolder = uploadConfigs.Value.TempFolder;
            _chunkSize = 1048576 * _uploadConfigs.ChunkSize;
            _tempPath = _env.WebRootPath + "/" + _uploadConfigs.FileUrl + _tempFolder;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseResult> GetAll(FilterSysFileVModel model)
        {
            var result = new ResponseResult();

            var data = await _sysFileRepo.GetAllPagination(
                model.PageNumber,
                model.PageSize,
                BuildPredicate(model),
                BuildOrderBy(model)
            );

            foreach (var entity in data.Records)
            {
                if (!string.IsNullOrEmpty(entity.Path) && !entity.Path.StartsWith(_jwtIssuerOptions.Audience, StringComparison.OrdinalIgnoreCase))
                {
                    entity.Path = $"{_jwtIssuerOptions.Audience?.TrimEnd('/')}{entity.Path}";
                }
            }

            var sysFileList = _mapper.Map<IEnumerable<SysFile>, IEnumerable<SysFileGetByIdVModel>>((IEnumerable<SysFile>)data.Records);
            data.Records = sysFileList;
            result.Data = data;

            return result;
        }

        private Expression<Func<SysFile, bool>> BuildPredicate(FilterSysFileVModel model)
        {
            return m =>
              (string.IsNullOrEmpty(model.Name) || m.Name.Contains(model.Name)) &&
              (string.IsNullOrEmpty(model.CreatedBy) || m.CreatedBy == model.CreatedBy) &&
              (string.IsNullOrEmpty(model.Path) || m.Path == model.Path) &&
              (model.IsActive == null || m.IsActive == model.IsActive) &&
              (string.IsNullOrEmpty(model.Type) || m.Type == model.Type);
        }


        private Expression<Func<SysFile, dynamic>> BuildOrderBy(FilterSysFileVModel model)
        {
            Expression<Func<SysFile, dynamic>> orderBy = x => x.Id;

            if (!string.IsNullOrEmpty(model.SortBy))
            {
                switch (model.SortBy.ToLower())
                {
                    case "name":
                        orderBy = x => x.Name;
                        break;
                    case "path":
                        orderBy = x => x.Path;
                        break;
                    case "type":
                        orderBy = x => x.Type;
                        break;
                }
            }
            return orderBy;
        }

        public override async Task<ResponseResult> Create(SysFileCreateVModel model)
        {
            var result = new ResponseResult();
            model.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.Name);
            model.Type = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.Type);
            model.Type = _medias.Contains(model.Type) ? model.Type : CommonConstants.FileType.Other;
            string yyyy = DateTime.Now.ToString("yyyy");
            string mm = DateTime.Now.ToString("MM");
            string envPath = _uploadConfigs.FileUrl + "/" + yyyy + "/" + mm;
            if (!Directory.Exists(_env.WebRootPath + "/" + envPath))
                Directory.CreateDirectory(_env.WebRootPath + "/" + envPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(model.Name);
            string fileExtension = Path.GetExtension(model.Name);
            string newFilePath = _env.WebRootPath + "/" + envPath + "/" + fileNameWithoutExtension + fileExtension;
            int count = 1;
            try
            {
                while (File.Exists(newFilePath))
                {
                    fileNameWithoutExtension = $"{Path.GetFileNameWithoutExtension(model.Name)}_{count}";
                    newFilePath = _env.WebRootPath + "/" + envPath + "/" + fileNameWithoutExtension + fileExtension;
                    count++;
                }
                File.Move(_tempPath + "/" + model.Name, newFilePath);
                model.Path = $"/{envPath}/{Path.GetFileName(newFilePath)}";
                var aspSystemFile = _mapper.Map<SysFile>(model);
                result = await _sysFileRepo.Create(aspSystemFile);
                aspSystemFile.CreatedDate = DateTime.Now;
                result.Data = Utilities.ConvertModel<SysFileCreateVModel>(result.Data);
                result.Data.Path = $"{GetBaseUrl()}{result.Data.Path}";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }

        public async Task FileChunks(FileChunk fileChunk)
        {
            if (Directory.Exists(_tempPath) == false)
                Directory.CreateDirectory(_tempPath);

            string newpath = _tempPath + "/" + fileChunk.FileName;
            using (FileStream fs = File.Create(newpath))
            {
                byte[] bytes = new byte[_chunkSize];
                int bytesRead = 0;
                if ((bytesRead = await fileChunk.File.OpenReadStream().ReadAsync(bytes, 0, bytes.Length)) > 0)
                    fs.Write(bytes, 0, bytesRead);
            }
        }

        private void MergeChunks(string chunk1, string chunk2)
        {
            FileStream fs1 = default!;
            FileStream fs2 = default!;
            try
            {
                fs1 = File.Open(chunk1, FileMode.Append);
                fs2 = File.Open(chunk2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                fs1?.Close();
                fs2?.Close();
                File.Delete(chunk2);
            }
        }

        public override async Task Update(SysFileUpdateVModel model)
        {
            var result = new ResponseResult();

            var entity = await _sysFileRepo.GetById(model.Id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            // Tạo entity từ model
            entity = _mapper.Map(model, entity);
            var baseUrl = _jwtIssuerOptions.Audience?.TrimEnd('/'); // Chắc chắn Base URL kết thúc bằng '/'
            var path = entity.Path;
            if (!string.IsNullOrEmpty(path))
            {
                Uri? uri;
                if (Uri.TryCreate(path, UriKind.Absolute, out uri))
                {
                    entity.Path = uri.PathAndQuery;
                }
            }

            result = await _sysFileRepo.Update(entity);
        }

        public async Task<ResponseResult> CreateBase64(SysFileCreateBase64VModel model)
        {
            string path = string.Empty;
            string type = string.Empty;
            if (!string.IsNullOrEmpty(model.Base64String))
            {
                string name = model.Name;
                var (uploadConfigs, typeOf) = ConvertBase64String.ConvertBase64ToImage(model.Base64String, $"{_env.WebRootPath}/{_uploadConfigs.ImageUrl}", name);
                path = uploadConfigs;
                type = typeOf;
            }
            // Map SysFileCreateBase64VModel sang SysFile
            var createdEntityBase64String = _mapper.Map<SysFileCreateBase64VModel, SysFile>(model);
            // Lấy sub domain www.local/upload/abc -> /upload/abc
            createdEntityBase64String.Path = path.Replace(_env.WebRootPath, "");
            createdEntityBase64String.Type = type.Trim();
            createdEntityBase64String.CreatedDate = DateTime.Now;
            // Thêm vào repo
            var result = await _sysFileRepo.Create(createdEntityBase64String);
            // Chuyển sang GetById để trả về đối tượng
            result.Data = _mapper.Map<SysFile, SysFileGetByIdVModel>(result.Data);
            result.Data.Path = $"{GetBaseUrl()}{result.Data.Path}";
            return result;
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            return $"{request?.Scheme}://{request?.Host}";
        }

        public async Task<ResponseResult> RemoveByUrl(string url)
        {
            ResponseResult result = new ResponseResult();
            url = HttpUtility.UrlDecode(url);
            string urlRemovedDomain = url.Replace(_jwtIssuerOptions.Audience ?? string.Empty, "");
            var entity = _sysFileRepo.AsQueryable().FirstOrDefault(x => x.Path.Contains(urlRemovedDomain));
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            string path = $"{_env.WebRootPath}/{entity.Path.Replace(_jwtIssuerOptions.Audience ?? string.Empty, "")}";
            if (File.Exists(path))
                File.Delete(path);
            result = await _sysFileRepo.Remove(entity.Id);

            return result;
        }

        public override async Task Remove(int id)
        {
            ResponseResult result = new ResponseResult();
            var entity = await _sysFileRepo.GetById(id);

            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            string path = $"{_env.WebRootPath}/{entity.Path.Replace(_jwtIssuerOptions.Audience ?? string.Empty, "")}";
            if (File.Exists(path))
                File.Delete(path);
            result = await _sysFileRepo.Remove(entity.Id);
        }

        private IImageFormat GetImageFormat(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return Image.DetectFormat(fs);
            }
        }

        public async Task<ResponseResult> UploadAvatar(FileChunk fileChunk)
        {
            var result = new ResponseResult();
            if (!Directory.Exists(_tempPath))
                Directory.CreateDirectory(_tempPath);

            FileInfo fi = new FileInfo(fileChunk.FileName);
            fileChunk.FileName = string.Format("{0}{1}", Guid.NewGuid().ToString(), fi.Extension);
            string newpath = Path.Combine(_tempPath, fileChunk.FileName);

            using (FileStream fs = File.Create(newpath))
            {
                byte[] bytes = new byte[_chunkSize];
                int bytesRead = 0;

                if ((bytesRead = await fileChunk.File.OpenReadStream().ReadAsync(bytes, 0, bytes.Length)) > 0)
                {
                    fs.Write(bytes, 0, bytesRead);
                }
            }

            IImageFormat imageFormat = GetImageFormat(newpath);

            var fileData = new SysFile()
            {
                Name = fileChunk.FileName,
                Path = Path.Combine(_env.WebRootPath, "Upload", "Files", fileChunk.FileName)
                    .Replace(_env.WebRootPath, "")
                    .Replace(Path.DirectorySeparatorChar, '/'),
                Type = imageFormat?.ToString() ?? string.Empty,
            };

            var repositoryresult = await _sysFileRepo.Create(fileData);
            result.Data = new
            {
                FilePath = Path.Combine(_env.WebRootPath, newpath.Replace(Path.DirectorySeparatorChar, '/'))
            };

            return result;
        }

        public async Task<ResponseResult> GetAllByType(string fileType, int pageSize, int pageNumber)
        {
            ResponseResult result = new ResponseResult();
            Pagination data = await _sysFileRepo.GetAllPagination(pageNumber, pageSize, x => x.Type == fileType, x => x.Id);
            foreach (var entity in data.Records)
            {
                GetAllEntry(entity);
            }
            foreach (var record in data.Records)
            {
                record.Path = CombineUrlWithDomain(record.Path);
            }
            data.Records = data.Records.Select(r => _mapper.Map<SysFile, SysFileGetAllVModel>(r));
            result.Data = data;
            result.Success = true;
            return result;
        }

        private string CombineUrlWithDomain(string path)
        {
            string domain = GetBaseUrl();
            return new Uri(new Uri(domain), path).ToString();
        }
    }
}
