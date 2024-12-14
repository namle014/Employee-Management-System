using AutoMapper;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;

namespace OA.Service
{
    public class SysApiService : BaseService<SysApi, SysApiCreateVModel, SysApiUpdateVModel, SysApiGetByIdVModel, SysApiGetAllVModel, SysApiExportVModel>, ISysApiService
    {
        private readonly IBaseRepository<SysApi> _sysApiRepo;
        private readonly IMapper _mapper;

        public SysApiService(IBaseRepository<SysApi> sysApiRepo, IMapper mapper) : base(sysApiRepo, mapper)
        {
            _sysApiRepo = sysApiRepo;
            _mapper = mapper;
        }

        public async Task<ResponseResult> Search(FilterSysAPIVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();
            var records = await _sysApiRepo.
                        Where(x =>
                            (model.IsActive == null || model.IsActive == x.IsActive) &&
                            (model.CreatedDate == null ||
                                    (x.CreatedDate.HasValue &&
                                    x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                    x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                    x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                            (string.IsNullOrEmpty(keyword) ||
                                    (x.ControllerName.ToLower().Contains(keyword) == true) ||
                                    (x.ActionName.ToLower().Contains(keyword) == true) ||
                                    (x.HttpMethod.ToLower().Contains(keyword) == true) ||
                                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword))
                        ));

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
                var list = new List<SysApiGetAllVModel>();
                foreach (var entity in records)
                {
                    var vmodel = _mapper.Map<SysApiGetAllVModel>(entity);
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

        public async Task<ExportStream> ExportFile(FilterSysAPIVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<SysApiExportVModel>>(result.Data?.Records);
            var exportData = ImportExportHelper<SysApiExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }
    }
}
