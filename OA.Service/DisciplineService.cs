using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
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

        public async Task<ResponseResult> GetTotalDisciplines(int years, int month)
        {
            var result = new ResponseResult();

            var previousMonth = month == 1 ? 12 : month - 1;
            var previousYear = month == 1 ? years - 1 : years;

            var firstDayOfMonth = new DateTime(years, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var firstDayOfPreviousMonth = new DateTime(previousYear, previousMonth, 1);
            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);

            var disciplines = await _dbContext.Discipline.Where(c => c.Date <= lastDayOfMonth && c.Date >= firstDayOfPreviousMonth).ToListAsync();

            var disciplinesInMonth = disciplines.Count(c =>
                c.Date <= lastDayOfMonth && c.Date >= firstDayOfMonth);

            var disciplinesInPreviousMonth = disciplines.Count(c =>
                c.Date <= lastDayOfPreviousMonth && c.Date >= firstDayOfPreviousMonth);

            var disciplinepercent = 0;
            if (disciplinesInPreviousMonth == 0)
            {
                disciplinepercent = 100;
            }
            else
            {
                disciplinepercent = (disciplinesInMonth - disciplinesInPreviousMonth) * 100 / disciplinesInPreviousMonth;
            }


            result.Data = new
            {
                TotalBenefit = disciplinesInMonth,
                BenefitPercent = disciplinepercent,
            };
            return result;
        }

        public Task<ResponseResult> GetTotalDisciplineByEmployeeInMonth(int year, int month)
        {
            var result = new ResponseResult();

            try
            {
                var disciplineStats = new List<object>();

                var firstDayOfMonth = new DateTime(year, month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var disciplines = _dbContext.Reward
                        .Where(r => r.Date >= firstDayOfMonth && r.Date <= lastDayOfMonth)
                        .GroupBy(r => r.UserId)
                        .Select(g => new
                        {
                            UserId = g.Key,
                            RewardCount = g.Count(),
                            FullName = _dbContext.AspNetUsers
                                .Where(u => u.Id == g.Key)
                                .Select(u => u.FullName)
                                .FirstOrDefault()
                        })
                        .OrderByDescending(r => r.RewardCount) // Sắp xếp giảm dần
                        .ToList();
                foreach (var discipline in disciplines)
                {
                    disciplineStats.Add(new
                    {
                        FullNames = discipline.FullName,
                        TotalRewards = discipline.RewardCount
                    });
                }

                result.Data = new
                {
                    DataDisciplines = disciplineStats
                };

            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return Task.FromResult(result); // Bọc result trong Task
        }

        public async Task<ResponseResult> GetDisciplineStatInYear(int year)
        {
            var result = new ResponseResult();
            try
            {
                var yearStats = new List<object>();

                for (int month = 1; month <= 12; month++)
                {

                    var firstDayOfMonth = new DateTime(year, month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);



                    var Discipline = await _dbContext.Discipline.Where(c => c.Date <= lastDayOfMonth && c.Date >= firstDayOfMonth).ToListAsync();

                    var disciplinesInMonth = Discipline.Count(c =>
                        c.Date <= lastDayOfMonth && c.Date >= firstDayOfMonth);




                    yearStats.Add(new
                    {
                        Month = month,
                        DisciplinesInMonths = disciplinesInMonth,
                    });
                }

                result.Data = new
                {
                    Year = year,
                    MonthlyStats = yearStats
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult> GetMeDisciplineInfo(DisciplineFilterVModel model ,int year)
        {
            var result = new ResponseResult();
            try
            {
                var userId = GlobalVariables.GlobalUserId != null ? GlobalVariables.GlobalUserId : string.Empty;
                var disciplineList = await _dbContext.Discipline.Where(x => x.IsActive && x.UserId == userId && x.Date.Year == year).ToListAsync();
                string? keyword = model.Keyword?.ToLower();
                var salaryAns = disciplineList.Where(x =>
                            (x.IsActive == model.IsActive) &&

                            (string.IsNullOrEmpty(keyword) ||
                                    x.Reason.ToLower().Contains(keyword) ||
                                    x.Date.ToString().ToLower().Contains(keyword) ||
                                    x.Money.ToString().ToLower().Contains(keyword) ||
                                    x.Note.ToLower().Contains(keyword)

                            ));
                if (model.IsDescending == false)
                {
                    salaryAns = string.IsNullOrEmpty(model.SortBy)
                            ? salaryAns.OrderBy(r => r.Date).ToList()
                            : salaryAns.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
                }
                else
                {
                    salaryAns = string.IsNullOrEmpty(model.SortBy)
                            ? salaryAns.OrderByDescending(r => r.Date).ToList()
                            : salaryAns.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
                }

                result.Data = new Pagination();

                var pagedRecords = salaryAns.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

                result.Data.Records = pagedRecords;
                result.Data.TotalRecords = salaryAns.Count();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
    }
}
