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
    public class RewardService : BaseService<Reward, RewardCreateVModel, RewardUpdateVModel, RewardGetByIdVModel, RewardGetAllVModel, RewardExportVModel>, IRewardService
    {
        private readonly IBaseRepository<Reward> _rewardRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public RewardService( ApplicationDbContext dbContext, IBaseRepository<Reward> rewardRepo, IMapper mapper) : base(rewardRepo, mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");

            _rewardRepo = rewardRepo;
            _mapper = mapper;

        }

        public async Task<ResponseResult> Search(RewardFilterVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();
            var records = await _rewardRepo.
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
                                    (x.Reason != null && x.Reason.ToLower().Contains(keyword))||
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
                var list = new List<RewardGetAllVModel>();
                foreach (var entity in records)
                {
                    var vmodel = _mapper.Map<RewardGetAllVModel>(entity);

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

        public async Task<ExportStream> ExportFile(RewardFilterVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<RewardExportVModel>>(result.Data?.Records);
            var exportData = ImportExportHelper<RewardExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }
        public override async Task Create(RewardCreateVModel model)
        {
            var rewardCreate =  _mapper.Map<RewardCreateVModel, Reward>(model);
            rewardCreate.Date = DateTime.Now;
            var createdResult = await _rewardRepo.Create(rewardCreate);
            //await base.Create(model);

            if (!createdResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "Object"));
            }
        }

        public async Task<ResponseResult> GetTotalRewards(int years, int month)
        {
            var result = new ResponseResult();

            var previousMonth = month == 1 ? 12 : month - 1;
            var previousYear = month == 1 ? years - 1 : years;

            var firstDayOfMonth = new DateTime(years, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var firstDayOfPreviousMonth = new DateTime(previousYear, previousMonth, 1);
            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);

            var rewards = await _dbContext.Reward.Where(c => c.Date <= lastDayOfMonth && c.Date >= firstDayOfPreviousMonth).ToListAsync();

            var rewardsInMonth = rewards.Count(c =>
                c.Date <= lastDayOfMonth && c.Date >= firstDayOfMonth);

            var rewardInPreviousMonth = rewards.Count(c =>
                c.Date <= lastDayOfPreviousMonth && c.Date >= firstDayOfPreviousMonth);

            var rewardpercent = 0;
            if (rewardInPreviousMonth == 0)
            {
                rewardpercent = 100;
            }
            else
            {
                rewardpercent = (rewardsInMonth - rewardInPreviousMonth) * 100 / rewardInPreviousMonth;
            }

            
            result.Data = new
            {
                TotalBenefit = rewardsInMonth,
                BenefitPercent = rewardpercent,
            };
            return result;
        }

        public Task<ResponseResult> GetTotalRewardByEmployeeInMonth(int year, int month)
        {
            var result = new ResponseResult();

            try
            {
                var rewardStats = new List<object>();

                var firstDayOfMonth = new DateTime(year, month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var rewards = _dbContext.Reward
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
                foreach (var reward in rewards)
                {
                    rewardStats.Add(new
                    {
                        FullNames = reward.FullName,
                        TotalRewards = reward.RewardCount
                    });
                }

                result.Data = new
                {
                    DataRewards = rewardStats
                };
                
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return Task.FromResult(result); // Bọc result trong Task
        }

        public async Task<ResponseResult> GetRewardStatInYear(int year)
        {
            var result = new ResponseResult();
            try
            {
                var yearStats = new List<object>();

                for (int month = 1; month <= 12; month++)
                {
                   
                    var firstDayOfMonth = new DateTime(year, month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    
                    

                    var rewards= await _dbContext.Reward.Where(c => c.Date <= lastDayOfMonth && c.Date >= firstDayOfMonth).ToListAsync();

                    var rewardsInMonth = rewards.Count(c =>
                        c.Date <= lastDayOfMonth && c.Date >= firstDayOfMonth);

                    
                    

                    yearStats.Add(new
                    {
                        Month = month,
                        RewardsInMonths = rewardsInMonth,
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
        public async Task<ResponseResult> GetMeRewardInfo(RewardFilterVModel model ,int year)
        {
            var result = new ResponseResult();
            try
            {
                var userId = GlobalVariables.GlobalUserId != null ? GlobalVariables.GlobalUserId : string.Empty;
                var rewardList = await _dbContext.Reward.Where(x => x.IsActive && x.UserId == userId && x.Date.Year == year).ToListAsync();
                string? keyword = model.Keyword?.ToLower();
                var salaryAns = rewardList.Where(x =>
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
