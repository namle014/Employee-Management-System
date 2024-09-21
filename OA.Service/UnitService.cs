using AutoMapper;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;

namespace OA.Service
{
    public class UnitService : BaseService<Unit, UnitCreateVModel, UnitUpdateVModel, UnitGetByIdVModel, UnitGetAllVModel, UnitExportVModel>, IUnitService
    {
        private readonly IBaseRepository<Unit> _unitRepo;
        private readonly IMapper _mapper;

        public UnitService(IBaseRepository<Unit> unitRepo, IMapper mapper) : base(unitRepo, mapper)
        {
            _unitRepo = unitRepo;
            _mapper = mapper;
        }

        public async Task<ResponseResult> Search(FilterGetAllVModel model)
        {
            var result = new ResponseResult();

            var records = await _unitRepo.Where(
                x =>
                (model.IsActive == null || x.IsActive == model.IsActive) &&
                (model.CreatedDate == null || (x.CreatedDate.HasValue && x.CreatedDate.Value.Year == model.CreatedDate.Value.Year && x.CreatedDate.Value.Month == model.CreatedDate.Value.Month && x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                (string.IsNullOrEmpty(model.CreatedBy) || x.CreatedBy == model.CreatedBy) &&
                (model.UpdatedDate == null || (x.UpdatedDate.HasValue && x.UpdatedDate.Value.Year == model.UpdatedDate.Value.Year && x.UpdatedDate.Value.Month == model.UpdatedDate.Value.Month && x.UpdatedDate.Value.Day == model.UpdatedDate.Value.Day)) &&
                (string.IsNullOrEmpty(model.UpdatedBy) || x.UpdatedBy == model.UpdatedBy)
            );


            IEnumerable<dynamic> data = records;
            if (!model.IsExport)
                data = records.Select(x => _mapper.Map<Unit, UnitGetAllVModel>(x));

            var keyword = model.Keyword?.ToLower();
            var list = data
                    .Where(x => string.IsNullOrEmpty(keyword) ||
                        (x.Name != null && x.Name.ToLower().Contains(keyword)) ||
                        (x.Note != null && x.Note.ToLower().Contains(keyword)) ||
                        (x.UnitCode != null && x.UnitCode.ToLower().Contains(keyword)) ||
                        (x.UpdatedBy != null && x.UpdatedBy.ToLower().Contains(keyword)) ||
                        (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword)));

            if (!model.IsDescending)
            {
                list = string.IsNullOrEmpty(model.SortBy)
                    ? list.OrderBy(r => r.Id).ToList()
                    : list.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                list = string.IsNullOrEmpty(model.SortBy)
                    ? list.OrderByDescending(r => r.Id).ToList()
                    : list.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            var totalRecords = list.Count();

            var paginatedRecords = list
                                    .Skip((model.PageNumber - 1) * model.PageSize)
                                    .Take(model.PageSize)
                                    .ToList();

            result.Data = new Pagination() { TotalRecords = totalRecords, Records = paginatedRecords };

            return result;
        }

        public override async Task Create(UnitCreateVModel model)
        {
            var entity = _mapper.Map<Unit>(model);

            var createdResult = await _unitRepo.Create(entity);
            if (createdResult.Success)
            {
                entity.UnitCode = "UN" + entity.Id.ToString("D6");

                var updatedResult = await _unitRepo.Update(entity);
                if (!updatedResult.Success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "Unit"));
                }
            }
            else
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "Unit"));
            }
        }

        public async Task<ExportStream> ExportFile(FilterGetAllVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<UnitExportVModel>>(result.Data?.Records);
            var exportData = ImportExportHelper<UnitExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }
    }
}
