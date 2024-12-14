using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;
using System.Text.RegularExpressions;


namespace OA.Service
{

    public class SalaryService : GlobalVariables, ISalaryService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Salary> _salary;
        private readonly IMapper _mapper;
        private string _nameService = "Salary";
        private DbSet<EmploymentContract> _employments;
        private readonly UserManager<AspNetUser> _userManager;

        public SalaryService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor, UserManager<AspNetUser> userManager) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _salary = dbContext.Set<Salary>();
            _employments = dbContext.Set<EmploymentContract>();
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task Create(SalaryCreateVModel model)
        {
            var entity = _mapper.Map<SalaryCreateVModel, Salary>(model);
            var idList = await _salary.Select(id => id.Id).ToListAsync();

            var highestId = idList.Select(id => new
            {
                originalId = id,
                numPart = int.TryParse(id.Substring(2),out int number)? number: -1 //nv001
            })
            .OrderByDescending(x => x.numPart).Select(x => x.originalId).FirstOrDefault();
            
            if (highestId != null)
            {
                if (highestId.Length > 2 && highestId.StartsWith("BL"))
                {
                    var newIdNumber = int.Parse(highestId.Substring(2)) + 1; 
                    entity.Id = "BL" + newIdNumber.ToString("D4"); 
                }
                else
                {
                    throw new InvalidOperationException("Invalid ID format in the database.");
                }
            }
            else
            {
                entity.Id = "BL0001";
            }
            entity.CreatedDate = DateTime.Now;
            entity.CreatedBy = GlobalUserName;
            entity.IsActive = false;
            _salary.Add(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
            }
        }
        public async Task<ResponseResult> GetAll()
        {
            var result = new ResponseResult();
            var query = _salary.AsQueryable();
            var salaryList = await query.Where(x=>x.IsActive).ToListAsync();
            var salaryGrouped = salaryList.GroupBy(x => x.UserId);
            var salaryListMapped = new List<SalaryGetAllVModel>();
            foreach (var group in salaryGrouped)
            {
                var userId = group.Key;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, $"UserId = {userId}"));
                }
                foreach (var item in group) {
                    var entityMapped = _mapper.Map<Salary, SalaryGetAllVModel>(item);
                    var totalBasicSalary = _employments.Where(x => x.UserId == userId && x.IsActive)
                                   .Sum(x => x.BasicSalary);
                    entityMapped.BasicSalary = totalBasicSalary;
                    var totalReward = _dbContext.Reward.Where(x => x.UserId == userId && x.IsActive).Sum(x=>x.Money);
                    entityMapped.Reward = totalReward;
                    var totalDiscipline = _dbContext.Discipline.Where(x => x.UserId == userId && x.IsActive).Sum(x => x.Money);
                    entityMapped.Discipline = totalDiscipline;
                    var timekeepingCount = _dbContext.Timekeeping
                        .Count(x => x.UserId == userId && x.IsActive
                                    && x.CheckInTime != TimeSpan.Zero && x.CheckOutTime != TimeSpan.Zero);
                    entityMapped.Timekeeping = timekeepingCount;
                    var totalBenefit = await (from bUser in _dbContext.BenefitUser
                                              join benefit in _dbContext.Benefit on bUser.BenefitId equals benefit.Id
                                              where (bUser.UserId == userId && benefit.IsActive)
                                              select benefit.BenefitContribution).SumAsync();
                    entityMapped.Benefit = totalBenefit;
                    var totalInsurance = await (from insuranceUser in _dbContext.InsuranceUser
                                               join insurance in _dbContext.Insurance on insuranceUser.InsuranceId equals insurance.Id
                                               where (insuranceUser.UserId == userId && insurance.IsActive)
                                               select insuranceUser.PaidInsuranceContribution).SumAsync();
                    var PITax = 0;

                    entityMapped.FullName = user.FullName;
                    salaryListMapped.Add(entityMapped); }
            }
            result.Data = salaryListMapped;
            return result;
        }

        public async Task<ResponseResult> GetById(string id)
        {
            var result = new ResponseResult();
            try
            {
                var entity = await _salary.FirstOrDefaultAsync(s => s.Id == id);
                if(entity == null)
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }

                result.Data = entity;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }

        public async Task<ResponseResult> Search(FilterSalaryVModel model)
        {
            var result = new ResponseResult();

            var query = _salary.AsQueryable();

            
            var userName = model.FullName;
            query = (from salary in _dbContext.Salary
                        join user in _dbContext.AspNetUsers on salary.UserId equals user.Id
            where (string.IsNullOrEmpty(model.FullName) || user.FullName.Contains(userName)) 
            && (!model.Month.HasValue || !model.Year.HasValue || (salary.CreatedDate.HasValue && salary.CreatedDate.Value.Month == model.Month && salary.CreatedDate.Value.Year == model.Year))
            && (!model.IsActive.HasValue || salary.IsActive == model.IsActive)
            select salary);
           

            var salaryList = await query.ToListAsync();
            var salaryGrouped = salaryList.GroupBy(x => x.UserId);
            var salaryListMapped = new List<SalaryGetAllVModel>();
            foreach(var group in salaryGrouped)
            {
                var user = await _userManager.FindByIdAsync(group.Key);
                if(user == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, $"UserId = {group.Key}"));
                }

                foreach (var item in group)
                {
                    var entityMapped = _mapper.Map<Salary, SalaryGetAllVModel>(item);
                    entityMapped.FullName = user.FullName;
                    salaryListMapped.Add(entityMapped);
                }
            }
            result.Data = salaryListMapped;
            return result;
        }

        public async Task Update(SalaryUpdateVModel model)
        {
            try
            {
                var entity = await _salary.FindAsync(model.Id);
                if(entity == null)
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }

                //_mapper.Map(model, entity);
                entity.TotalSalary = model.TotalSalary;
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedBy = GlobalUserName;
                _salary.Update(entity);

                bool success = await _dbContext.SaveChangesAsync() > 0;
                
                if (!success)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
        }

        public async Task Remove(string id)
        {
            var entity = await _salary.FindAsync(id);
            if(entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            _salary.Remove(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task ChangeStatus(string id)
        {
            try
            {
                var entity = await _salary.FindAsync(id);
                if (entity != null)
                {
                    entity.UpdatedDate = DateTime.Now;
                    entity.UpdatedBy = GlobalUserName;
                    entity.IsActive = true;

                    _salary.Update(entity);
                    var result = await _dbContext.SaveChangesAsync() > 0;
                    if (!result)
                    {
                        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorChangeStatus, _nameService));
                    }
                }
                else
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
        }

        public Task<ResponseResult> GetIncomeInMonth(int year, int month)
        {
            var result = new ResponseResult();
            var query = _salary.AsQueryable();
            string period = $"{year}-{month.ToString("D2")}";
            var total =  query.Where(x => x.IsActive && x.PayrollPeriod == period).Sum(x => (decimal?)x.TotalSalary) ?? 0;
            int bMonth = 0;
            int bYear = 0;
            if(month == 1)
            {
                bMonth = 12;
                bYear = year - 1;
            }
            else
            {
                bMonth = month - 1;
                bYear = year;
            }
            string bPeriod = $"{bYear}-{bMonth.ToString("D2")}";
            var bTotal = query.Where(x => x.IsActive && x.PayrollPeriod == bPeriod).Sum(x => (decimal?)x.TotalSalary) ?? 0;

            float? percentage = 0;
            if (bTotal != 0)
            {
                percentage = (float)(((total - bTotal) / bTotal) * 100);
            }
            else percentage = null;

            result.Data = new
            {
                TotalIncome = total,
                PercentageChange = percentage
            };

            return Task.FromResult(result);
        }

        public Task<ResponseResult> GetYearIncome(int year)
        {
            var result = new ResponseResult();
            var query = _salary.AsQueryable();
            var months = Enumerable.Range(1,12).Select(m => m.ToString("D2")).ToArray();
            var dbData = query
                .Where(x => x.IsActive && x.PayrollPeriod != null && x.PayrollPeriod.StartsWith(year.ToString()))
                .GroupBy(x => x.PayrollPeriod.Substring(5, 2))
                .Select(g => new {
                    month = g.Key,
                    total = g.Sum(x => (decimal?)x.TotalSalary) ?? 0})
                .ToArray();

            var totalSalaries = months.Select(m => new
            {
                month = m,
                total = dbData.FirstOrDefault(x => x.month == m)?.total ?? 0
            }).ToArray(); 

            var bYear = year - 1;

            var bdbData = query
                .Where(x => x.IsActive && x.PayrollPeriod != null && x.PayrollPeriod.StartsWith(bYear.ToString()))
                .GroupBy(x => x.PayrollPeriod.Substring(5, 2))
                .Select(g => new {
                    month = g.Key,
                    total = g.Sum(x => (decimal?)x.TotalSalary) ?? 0
                })
                .ToArray();

            var bTotalSalaries = months.Select(m => new
            {
                month = m,
                total = bdbData.FirstOrDefault(x => x.month == m)?.total ?? 0
            }).ToArray();

            result.Data = new
            {
                yearList = totalSalaries,
                bYearList = bTotalSalaries
            };

            return Task.FromResult(result);
        }
    }
}
