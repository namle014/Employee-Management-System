using AutoMapper;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
namespace OA.Service
{
    public class SysConfigurationService : BaseService<SysConfiguration, SysConfigurationCreateVModel, SysConfigurationUpdateVModel, SysConfigurationGetByIdVModel, SysConfigurationGetAllVModel, SysConfigurationExportVModel>, ISysConfigurationService
    {
        private readonly IBaseRepository<SysConfiguration> _sysConfigRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public SysConfigurationService(IBaseRepository<SysConfiguration> sysConfigRepo, ApplicationDbContext dbContext, IMapper mapper) : base(sysConfigRepo, mapper)
        {
            _sysConfigRepo = sysConfigRepo;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResponseResult> GetByConfigTypeKey(string type, string key)
        {
            var result = new ResponseResult();
            var entity = (await _sysConfigRepo.Where(x => x.Type == type && x.Key == key)).FirstOrDefault();

            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            result.Data = _mapper.Map<SysConfiguration, SysConfigurationGetByIdVModel>(entity);

            return result;
        }

        public async Task<ResponseResult> Search(FilterSysConfigurationVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();

            var records = await _sysConfigRepo.Where(x =>
                        (x.IsActive == model.IsActive) &&
                        (model.CreatedDate == null ||
                                (x.CreatedDate.HasValue &&
                                x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                        (string.IsNullOrEmpty(keyword) ||
                                x.Key.ToLower().Contains(keyword) ||
                                x.Description.ToLower().Contains(keyword) ||
                                x.Type.ToLower().Contains(keyword) ||
                                x.Value.ToLower().Contains(keyword) ||
                                (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword))
                        ));

            if (model.IsDescending == false)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderBy(r => r.CreatedDate).ToList()
                        : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderByDescending(r => r.CreatedDate).ToList()
                        : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            result.Data = new Pagination();

            if (!model.IsExport)
            {
                var list = new List<SysConfigurationGetAllVModel>();
                foreach (var entity in records)
                {
                    var vmodel = _mapper.Map<SysConfigurationGetAllVModel>(entity);
                    list.Add(vmodel);
                }
                var pagedRecords = list.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

                result.Data.Records = pagedRecords;
                result.Data.TotalRecords = list.Count;
            }
            else
            {
                var pagedRecords = records.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

                result.Data.Records = pagedRecords;
                result.Data.TotalRecords = records.ToList().Count;
            }

            return result;
        }

        public async Task ChangeStatusMany(SysConfigurationChangeStatusManyVModel model)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.Ids.Any())
                    {
                        foreach (var id in model.Ids)
                        {
                            var entity = await _sysConfigRepo.GetById(id);
                            if (entity == null)
                            {
                                throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, id));
                            }
                            entity.IsActive = !entity.IsActive;
                            var updatedResult = await _sysConfigRepo.Update(entity);
                            if (!updatedResult.Success)
                            {
                                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, id));
                            }
                        }

                        await transaction.CommitAsync();
                    }
                    else
                    {
                        throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException(ex.Message);
                }
            }
        }

        public async Task<ExportStream> ExportFile(FilterSysConfigurationVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<SysConfigurationExportVModel>>(result.Data?.Records);
            var exportData = ImportExportHelper<SysConfigurationExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }
    }
}
