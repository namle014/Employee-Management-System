using AutoMapper;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
//using Twilio.TwiML.Voice;

namespace OA.Service
{
    public class WorkingRulesService : BaseService<WorkingRules, WorkingRulesCreateVModel, WorkingRulesUpdateVModel, WorkingRulesGetByIdVModel, WorkingRulesGetAllVModel, WorkingRulesExportVModel>, IWorkingRulesService
    {
        private readonly IBaseRepository<WorkingRules> _workingrulesRepo;
        private readonly IMapper _mapper;

        public WorkingRulesService(IBaseRepository<WorkingRules> workingrulesRepo, IMapper mapper) : base(workingrulesRepo, mapper)
        {
            _workingrulesRepo = workingrulesRepo;
            _mapper = mapper;
        }

        public async Task<ResponseResult> Search(WorkingRulesFilterVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();
            var records = await _workingrulesRepo.
                        Where(x =>
                            (model.IsActive == null || model.IsActive == x.IsActive) &&
                            (model.CreatedDate == null ||
                                    (x.CreatedDate.HasValue &&
                                    x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                    x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                    x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                            (string.IsNullOrEmpty(keyword) ||
                                    (x.Note != null && x.Note.ToLower().Contains(keyword)) ||
                                    (x.Name != null && x.Name.ToLower().Contains(keyword))||
                                    (x.Content != null && x.Content.ToLower().Contains(keyword)) ||
                                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword))||
                                    (x.UpdatedBy != null && x.UpdatedBy.ToLower().Contains(keyword))));

            if (!model.IsDescending)
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

            if (!model.IsExport)
            {
                var list = new List<WorkingRulesGetAllVModel>();
                foreach (var entity in records)
                {
                    var vmodel = _mapper.Map<WorkingRulesGetAllVModel>(entity);
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

        public async Task<ExportStream> ExportFile(WorkingRulesFilterVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<WorkingRulesExportVModel>>(result.Data?.Records);
            var exportData = ImportExportHelper<WorkingRulesExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }
        //public override async Task Create(WorkingRulesCreateVModel model)
        //{
        //    var rewardCreate =  _mapper.Map<RewardCreateVModel, Reward>(model);
        //    rewardCreate.Date = DateTime.Now;
        //    var createdResult = await _workingrulesRepo.Create(rewardCreate);
        //    //await base.Create(model);

        //    if (!createdResult.Success)
        //    {
        //        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "Object"));
        //    }
        //}
    }
}
