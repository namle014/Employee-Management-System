using AutoMapper;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Infrastructure.SQL;
using OA.Service.Helpers;

namespace OA.Service
{
    public class WorkShiftService : BaseService<WorkShifts, WorkShiftCreateVModel, WorkShiftUpdateVModel, WorkShiftGetByIdVModel, WorkShiftGetAllVModel, WorkShiftExportVModel>, IWorkShiftService
    {
        private readonly IBaseRepository<WorkShifts> _workShiftRepo;
        private readonly IMapper _mapper;
        private static BaseConnection _dbConnectSQL = BaseConnection.Instance();
        private readonly ApplicationDbContext _dbContext;
        public WorkShiftService(IBaseRepository<WorkShifts> workShiftRepo, ApplicationDbContext dbContext,
                                    IMapper mapper) : base(workShiftRepo, mapper)
        {
            _workShiftRepo = workShiftRepo;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResponseResult> Search(FilterWorkShiftVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();

            var records = await _workShiftRepo.Where(x =>
                        (x.IsActive == model.IsActive) &&
                        (model.CreatedDate == null ||
                                (x.CreatedDate.HasValue &&
                                x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                        (string.IsNullOrEmpty(keyword) ||
                                x.Description.ToLower().Contains(keyword) ||
                                x.ShiftName.ToLower().Contains(keyword) ||
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
                var list = new List<WorkShiftGetAllVModel>();
                foreach (var entity in records)
                {
                    var vmodel = _mapper.Map<WorkShiftGetAllVModel>(entity);
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
                            var entity = await _workShiftRepo.GetById(id);
                            if (entity == null)
                            {
                                throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, id));
                            }
                            entity.IsActive = !entity.IsActive;
                            var updatedResult = await _workShiftRepo.Update(entity);
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

        public override async Task Create(WorkShiftCreateVModel model)
        {
            if (!(await CheckTimeValid(model.StartTime, model.EndTime)))
            {
                throw new BadRequestException("Time not valid!");
            }

            await base.Create(model);
        }

        public override async Task Update(WorkShiftUpdateVModel model)
        {
            if (!(await CheckTimeValid(model.StartTime, model.EndTime, model.Id)))
            {
                throw new BadRequestException("Time not valid!");
            }

            await base.Update(model);
        }

        public async Task<bool> CheckTimeValid(TimeSpan? startTime, TimeSpan? endTime, long? id = null)
        {
            if (!startTime.HasValue && !endTime.HasValue)
            {
                return true;
            }
            if (!startTime.HasValue || !endTime.HasValue)
            {
                return false;
            }

            var data = await _workShiftRepo.GetAllPagination(1, CommonConstants.ConfigNumber.pageSizeDefault);

            if (endTime < startTime)
            {
                endTime = endTime.Value.Add(new TimeSpan(24, 0, 0));
            }

            foreach (var shift in data.Records)
            {
                if (id != null && id == shift.Id)
                {
                    continue;
                }

                if (shift.StartTime == null || shift.EndTime == null)
                {
                    continue;
                }

                TimeSpan shiftEndTime = shift.EndTime;
                if (shift.EndTime < shift.StartTime)
                {
                    if (shift.StartTime <= startTime || shift.EndTime >= endTime)
                    {
                        return false;
                    }
                    shiftEndTime = shiftEndTime.Add(new TimeSpan(24, 0, 0));
                }

                if ((shift.StartTime <= startTime && shiftEndTime > startTime) ||
                    (shift.StartTime < endTime && shiftEndTime >= endTime) ||
                    (shift.StartTime >= startTime && shiftEndTime <= endTime))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<ExportStream> ExportFile(FilterWorkShiftVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            if (result.Data != null)
            {
                var records = _mapper.Map<IEnumerable<WorkShiftExportVModel>>(result.Data.Records);
                var exportData = ImportExportHelper<WorkShiftExportVModel>.ExportFile(exportModel, records);
                return exportData;
            }
            else
            {
                throw new BadRequestException(MsgConstants.ErrorMessages.ErrorExport);
            }
        }

    }
}
