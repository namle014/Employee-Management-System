﻿using AutoMapper;
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
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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
        private readonly string _avatarPath;

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
            _chunkSize = 1048576 * _uploadConfigs.ChunkSize; // Kích thước của mỗi chunk (1MB * chunk size)
            _tempPath = Path.Combine(_env.WebRootPath, _uploadConfigs.FileUrl, _tempFolder);
            _avatarPath = Path.Combine(_env.WebRootPath, "avatars"); // Tạo đường dẫn đến thư mục avatars
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ResponseResult> GetAll(FilterSysFileVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();

            var records = await _sysFileRepo.
                            Where(x =>
                                (model.IsActive == null || x.IsActive == model.IsActive) &&
                                (model.CreatedDate == null ||
                                    (x.CreatedDate.HasValue &&
                                    x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                    x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                    x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                                (string.IsNullOrEmpty(keyword) ||
                                    x.Type.ToLower().Contains(keyword) ||
                                    x.Name.ToLower().Contains(keyword) ||
                                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword))
                                )
                            );

            if (model.IsDescending == true)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderBy(r => r.Id).ToList()
                        : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderByDescending(r => r.Id).ToList()
                        : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            result.Data = new Pagination();

            var list = new List<SysFileGetByIdVModel>();
            foreach (var entity in records)
            {
                if (!string.IsNullOrEmpty(entity.Path) && !string.IsNullOrEmpty(_jwtIssuerOptions.Audience) &&
                        !entity.Path.StartsWith(_jwtIssuerOptions.Audience, StringComparison.OrdinalIgnoreCase))
                {
                    entity.Path = $"{_jwtIssuerOptions.Audience?.TrimEnd('/')}{entity.Path}";
                }

                var vmodel = _mapper.Map<SysFileGetByIdVModel>(entity);
                list.Add(vmodel);
            }

            var pagedRecords = list.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

            result.Data.Records = pagedRecords;
            result.Data.TotalRecords = list.Count;

            return result;
        }

        public override async Task Create(SysFileCreateVModel model)
        {
            // Ensure the model name is formatted correctly
            model.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.Name);
            model.Type = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.Type);
            model.Type = _medias.Contains(model.Type) ? model.Type : CommonConstants.FileType.Other;

            // Create directory structure based on the current date
            string yyyy = DateTime.Now.ToString("yyyy");
            string mm = DateTime.Now.ToString("MM");
            string envPath = Path.Combine(_uploadConfigs.FileUrl, yyyy, mm);

            // Ensure the directory exists
            string fullPath = Path.Combine(_env.WebRootPath, envPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            // Prepare the new file path
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(model.Name);
            string fileExtension = Path.GetExtension(model.Name);
            string newFilePath = Path.Combine(fullPath, fileNameWithoutExtension + fileExtension);

            // Handle duplicate file names
            int count = 1;
            while (File.Exists(newFilePath))
            {
                fileNameWithoutExtension = $"{Path.GetFileNameWithoutExtension(model.Name)}_{count}";
                newFilePath = Path.Combine(fullPath, fileNameWithoutExtension + fileExtension);
                count++;
            }

            try
            {
                // Move the file from the temporary path to the new path
                string tempFilePath = Path.Combine(_tempPath, model.Name);
                if (!File.Exists(tempFilePath))
                {
                    throw new FileNotFoundException($"Temporary file not found: {tempFilePath}");
                }

                File.Move(tempFilePath, newFilePath);

                // Map the model to SysFile and save to repository
                var entity = _mapper.Map<SysFile>(model);

                // Update the model path for database storage
                entity.Path = $"/{envPath}/{Path.GetFileName(newFilePath)}";

                entity.CreatedDate = DateTime.Now; // Set created date here
                var createdResult = await _sysFileRepo.Create(entity);

                if (!createdResult.Success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "File"));
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
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

        public override async Task Update(SysFileUpdateVModel model)
        {
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

            var updatedResult = await _sysFileRepo.Update(entity);

            if (!updatedResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "File"));
            }
        }

        public async Task CreateBase64(SysFileCreateBase64VModel model)
        {
            string path = string.Empty;
            string type = string.Empty;
            if (!string.IsNullOrEmpty(model.Base64String))
            {
                string name = model.Name;
                var convertSolve = ConvertBase64String.ConvertBase64ToImage(model.Base64String, $"{_env.WebRootPath}/{_uploadConfigs.ImageUrl}", name);
                path = convertSolve.FilePath;
                type = convertSolve.FileType;
            }

            var createdEntityBase64String = new SysFile()
            {
                Name = model.Name,
                Path = path.Replace(_env.WebRootPath, ""),
                Type = type.Trim()
            };
            // Lấy sub domain www.local/upload/abc -> /upload/abc
            // Thêm vào repo
            var createdResult = await _sysFileRepo.Create(createdEntityBase64String);
            if (!createdResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "File"));
            }
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            return $"{request?.Scheme}://{request?.Host}";
        }

        public async Task RemoveByUrl(string url)
        {
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

            var removedResult = await _sysFileRepo.Remove(entity.Id);
            if (!removedResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, "SysFile"));
            }
        }

        public override async Task Remove(int id)
        {
            var entity = await _sysFileRepo.GetById(id);

            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            string path = $"{_env.WebRootPath}/{entity.Path.Replace(_jwtIssuerOptions.Audience ?? string.Empty, "")}";
            if (File.Exists(path))
                File.Delete(path);

            var removedResult = await _sysFileRepo.Remove(entity.Id);
            if (!removedResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, "SysFile"));
            }
        }

        private IImageFormat GetImageFormat(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return Image.DetectFormat(fs);
            }
        }

        public async Task UploadAvatar(FileChunk fileChunk)
        {
            if (!Directory.Exists(_avatarPath))
                Directory.CreateDirectory(_avatarPath);

            FileInfo fi = new FileInfo(fileChunk.FileName);
            string newFileName = $"{Guid.NewGuid()}{fi.Extension}"; // This creates a unique filename for storage
            string newpath = Path.Combine(_avatarPath, newFileName);

            using (FileStream fs = new FileStream(newpath, FileMode.Create))
            {
                byte[] bytes = new byte[_chunkSize];
                int bytesRead;
                try
                {
                    using (var fileStream = fileChunk.File.OpenReadStream())
                    {
                        if (fileChunk.File.Length == 0)
                        {
                            throw new BadRequestException("File không chứa dữ liệu.");
                        }

                        while ((bytesRead = await fileStream.ReadAsync(bytes, 0, bytes.Length)) > 0)
                        {
                            await fs.WriteAsync(bytes, 0, bytesRead);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new BadRequestException($"Đã xảy ra lỗi khi đọc file: {ex.Message}");
                }
            }

            IImageFormat imageFormat = GetImageFormat(newpath);
            if (imageFormat == null)
            {
                File.Delete(newpath);
                throw new BadRequestException("File không phải là định dạng ảnh hợp lệ.");
            }

            var fileData = new SysFile()
            {
                Name = fileChunk.FileName,
                Path = $"/avatars/{newFileName}",
                Type = imageFormat?.ToString() ?? string.Empty,
            };

            var createdResult = await _sysFileRepo.Create(fileData);

            if (!createdResult.Success)
            {
                throw new BadRequestException($"Failed to create file {fileData.Name}");
            }
        }
    }
}
