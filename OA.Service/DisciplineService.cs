using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;
//using Twilio.TwiML.Voice;

namespace OA.Service
{
    public class DisciplineService : BaseService<Discipline, DisciplineCreateVModel, DisciplineUpdateVModel, DisciplineGetByIdVModel, DisciplineGetAllVModel, DisciplineExportVModel>, IDisciplineService
    {
        private readonly IBaseRepository<Discipline> _disciplineRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;


        public DisciplineService(ApplicationDbContext dbContext, IBaseRepository<Discipline> disciplineRepo, IMapper mapper) : base(disciplineRepo, mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");

            _disciplineRepo = disciplineRepo;
            _mapper = mapper;
        }

        public async Task<ResponseResult> Search(DisciplineFilterVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();
            var records = await _disciplineRepo.
                        Where(x =>
                            (model.IsActive == null || model.IsActive == x.IsActive) &&
                            (model.CreatedDate == null ||
                                    (x.CreatedDate.HasValue &&
                                    x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                    x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                    x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                            (string.IsNullOrEmpty(keyword) ||
                                    (x.UserId.ToLower().Contains(keyword) == true) ||
                                    (x.Note != null && x.Note.ToLower().Contains(keyword)) ||
                                    (x.Reason != null && x.Reason.ToLower().Contains(keyword)) ||
                                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword)) ||
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
                var list = new List<DisciplineGetAllVModel>();
                foreach (var entity in records)
                {
                    var vmodel = _mapper.Map<DisciplineGetAllVModel>(entity);
                    var userId = entity.UserId;
                    var usertable = await _dbContext.AspNetUsers
                    .Where(x => x.Id == userId).ToListAsync();
                    vmodel.FullName = usertable[0].FullName;
                    int? departmentId = usertable[0].DepartmentId;
                    var departmentName = await _dbContext.Department.Where(x => x.Id == departmentId).Select(x => x.Name).FirstOrDefaultAsync();
                    vmodel.Department = departmentName;
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

        public async Task<ExportStream> ExportFile(DisciplineFilterVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<DisciplineExportVModel>>(result.Data?.Records);
            var exportData = ImportExportHelper<DisciplineExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }
        public override async Task Create(DisciplineCreateVModel model)
        {
            var disciplineCreate =  _mapper.Map<DisciplineCreateVModel, Discipline>(model);
            disciplineCreate.Date = DateTime.Now;
            var createdResult = await _disciplineRepo.Create(disciplineCreate);
            //await base.Create(model);

            if (!createdResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "Object"));
            }
        }
    }
}
