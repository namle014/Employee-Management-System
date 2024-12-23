﻿using AngleSharp.Css;
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
using System.Collections.Generic;
using System.Globalization;
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

        public async Task Create()
        {
            var query = _userManager.Users.Where(x => x.IsActive);
            var userList = await query.ToListAsync();
            var idList = await _salary.Select(id => id.Id).ToListAsync();

            var highestId = idList.Select(id => new
            {
                originalId = id,
                numPart = int.TryParse(id.Substring(2), out int number) ? number : -1 //nv001
            })
            .OrderByDescending(x => x.numPart).Select(x => x.originalId).FirstOrDefault();

            int currentMaxNumber = 1;
            if (highestId != null && highestId.Length > 2 && highestId.StartsWith("BL"))
            {
                currentMaxNumber = int.Parse(highestId.Substring(2)) + 1;
            }

            var salaryList = new List<Salary>();
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var morningTimeConfig = _dbContext.SysConfigurations.FirstOrDefault(x => x.Key == "MORNING_TIME")?.Value;
            var lunchBreakConfig = _dbContext.SysConfigurations.FirstOrDefault(x => x.Key == "LUNCH_BREAK")?.Value;
            var afternoonTimeConfig = _dbContext.SysConfigurations.FirstOrDefault(x => x.Key == "AFTERNOON_TIME")?.Value;
            var quittingTimeConfig = _dbContext.SysConfigurations.FirstOrDefault(x => x.Key == "QUITTING_TIME")?.Value;

            var morningTime = DateTime.Today.Add(TimeSpan.Parse(morningTimeConfig)).TimeOfDay;
            var lunchBreak = DateTime.Today.Add(TimeSpan.Parse(lunchBreakConfig)).TimeOfDay;
            var afternoonTime = DateTime.Today.Add(TimeSpan.Parse(afternoonTimeConfig)).TimeOfDay;
            var quittingTime = DateTime.Today.Add(TimeSpan.Parse(quittingTimeConfig)).TimeOfDay;


            foreach (var user in userList)
            {
                var userId = user.Id;
                var totalBasicSalary = _employments.Where(x => x.UserId == userId && x.IsActive)
                                   .Sum(x => x.BasicSalary);
                var workingDays = 0;

                for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                {
                    DateTime currentDate = new DateTime(year, month, day);

                    if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        workingDays++;
                    }
                }


                var totalHours = _dbContext.Timekeeping
                    .Where(x => x.UserId == userId
                                && x.IsActive
                                && x.CheckInTime >= morningTime
                                && x.CheckOutTime <= quittingTime
                                && x.Date.Month == month
                                && x.Date.Year == year)
                    .AsEnumerable()
                    .Sum(x => ((x.CheckOutTime - afternoonTime) + (lunchBreak - x.CheckInTime)).TotalHours);


                var workdays = Math.Round(totalHours / 8, 2);

                var dailyWage = (double)totalBasicSalary / workingDays * workdays;


                var totalReward = _dbContext.Reward.Where(x => x.UserId == userId && x.IsActive).Sum(x => x.Money) ?? 0;
                var totalDiscipline = _dbContext.Discipline.Where(x => x.UserId == userId && x.IsActive).Sum(x => x.Money);
                var totalBenefit = await (from bUser in _dbContext.BenefitUser
                                          join benefit in _dbContext.Benefit on bUser.BenefitId equals benefit.Id
                                          where (bUser.UserId == userId && benefit.IsActive)
                                          select benefit.BenefitContribution).SumAsync();

                var insuranceList = (from insuranceUser in _dbContext.InsuranceUser
                                     join insurance in _dbContext.Insurance on insuranceUser.InsuranceId equals insurance.Id
                                     where (insuranceUser.UserId == userId && insurance.IsActive && insuranceUser.Status != "Đã đóng")
                                     select insuranceUser);

                var bhtn = Convert.ToDouble(_dbContext.SysConfigurations.FirstOrDefault(x => x.Key == "MD_BHTN")?.Value);
                double totalInsurance = 0;
                double maxBaseSalary = 20 * 2340000;
                double maxBasicSalary = 20 * bhtn;

                foreach (var ins in insuranceList)
                {
                    var rate = ins.EmployeeContributionRate ?? 0;
                    var maxSalary = (ins.InsuranceId == "BHTN") ? maxBasicSalary : maxBaseSalary;
                    var PaidInsuranceContribution = Math.Min(dailyWage, maxSalary) * (double)rate;

                    var eIns = await _dbContext.InsuranceUser.FirstOrDefaultAsync(x => x.InsuranceId == ins.InsuranceId);
                    if (eIns != null)
                    {
                        eIns.PaidInsuranceContribution = (decimal)PaidInsuranceContribution;
                        _dbContext.InsuranceUser.Update(eIns);
                        await _dbContext.SaveChangesAsync();
                    }

                    totalInsurance += PaidInsuranceContribution;
                }

                var relative = (await _userManager.FindByIdAsync(userId))?.EmployeeDependents;

                var taxableIncome = (dailyWage) > 11000000 ? ((decimal)dailyWage + totalReward - totalDiscipline + totalBenefit - 4400000 * relative - (decimal)totalInsurance - 11000000) : 0;
                double PITax = getPITax(taxableIncome);

                var model = new SalaryCreateVModel
                {
                    UserId = userId,
                    Date = DateTime.Now.Date,
                };
                var entity = _mapper.Map<SalaryCreateVModel, Salary>(model);
                entity.ProRatedSalary = (decimal)dailyWage;
                entity.PITax = (decimal)PITax;
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = GlobalUserName;
                entity.IsActive = true;
                entity.PayrollPeriod = $"{month.ToString("D2")}-{year}";
                entity.Id = $"BL{currentMaxNumber.ToString("D4")}";
                entity.IsPaid = false;
                entity.TotalSalary = (decimal)dailyWage + totalReward - totalDiscipline + totalBenefit - (decimal)totalInsurance - (decimal)PITax;
                entity.SalaryPayment = (decimal)dailyWage + totalReward - totalDiscipline + totalBenefit;
                salaryList.Add(entity);
                currentMaxNumber++;

            }
            await _salary.AddRangeAsync(salaryList);


            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
            }
        }

        public double getPITax(decimal? taxableIncome)
        {
            double PITax = 0;
            if (taxableIncome == null)
            {
                return 0;
            }
            else if (taxableIncome <= 5000000)
            {
                PITax = 5 / 100f * (double)taxableIncome;
            }
            else if (taxableIncome <= 10000000)
            {
                PITax = 10 / 100f * (double)taxableIncome - 250000;
            }
            else if (taxableIncome <= 18000000)
            {
                PITax = 15 / 100f *(double)taxableIncome - 750000;
            }
            else if (taxableIncome <= 32000000)
            {
                PITax = 20 / 100f * (double)taxableIncome - 1650000;
            }
            else if (taxableIncome <= 52000000)
            {
                PITax = 25 / 100f * (double)taxableIncome - 3250000;
            }
            else if (taxableIncome <= 80000000)
            {
                PITax = 30 / 100f * (double)taxableIncome - 5850000;
            }
            else
            {
                PITax = 35 / 100f * (double)taxableIncome - 9850000;
            }
            return PITax;
        }
        public async Task<ResponseResult> GetAll(SalaryFilterVModel model, string period)
        {
                var result = new ResponseResult();
            var query = _salary.AsQueryable();
            var salaryList = await query.Where(x => x.IsActive && x.PayrollPeriod == period).ToListAsync();
            var salaryGrouped = salaryList.GroupBy(x => x.UserId);
            var salaryListMapped = new List<SalaryGetAllVModel>();
            var month = Convert.ToInt32(period.Substring(0, 2));
            var year = Convert.ToInt32(period.Substring(3, 4));
            foreach (var group in salaryGrouped)
            {
                var userId = group.Key;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, $"UserId = {userId}"));
                }
                foreach (var item in group)
                {
                    var entityMapped = _mapper.Map<Salary, SalaryGetAllVModel>(item);
                    entityMapped.BasicSalary = item.ProRatedSalary;
                    var totalReward = _dbContext.Reward.Where(x => x.UserId == userId && x.IsActive).Sum(x => x.Money) ?? 0;
                    entityMapped.Reward = totalReward;
                    var totalDiscipline = _dbContext.Discipline.Where(x => x.UserId == userId && x.IsActive).Sum(x => x.Money);
                    entityMapped.Discipline = totalDiscipline;
                    var timekeepingCount = _dbContext.Timekeeping
                        .Count(x => x.UserId == userId && x.IsActive
                                    && x.CheckInTime != TimeSpan.Zero && x.CheckOutTime != TimeSpan.Zero && x.Date.Year == year && x.Date.Month == month);
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
                    var PITax = item.PITax;

                    entityMapped.FullName = user.FullName;
                    salaryListMapped.Add(entityMapped);
                }
            }
            string? keyword = model.Keyword?.ToLower();
            var salaryAns = salaryListMapped.Where(x =>
                        (x.IsActive == model.IsActive) &&

                        (string.IsNullOrEmpty(keyword) ||
                                x.FullName.ToLower().Contains(keyword) ||
                                x.Benefit.ToString().ToLower().Contains(keyword) ||
                                x.Discipline.ToString().ToLower().Contains(keyword) ||
                                x.Reward.ToString().ToLower().Contains(keyword) ||
                                x.BasicSalary.ToString().ToLower().Contains(keyword) ||
                                x.Insurance.ToString().ToLower().Contains(keyword) ||
                                x.Timekeeping.ToString().ToLower().Contains(keyword) ||
                                x.PITax.ToString().ToLower().Contains(keyword) ||
                                x.PayrollPeriod.ToString().ToLower().Contains(keyword)
                        )); ;
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

            return result;
        }

        public async Task<ResponseResult> GetById(string id)
        {
            var result = new ResponseResult();
            try
            {
                var entity = await _salary.FirstOrDefaultAsync(s => s.Id == id);
                if (entity == null)
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

        //public async Task<ResponseResult> Search(FilterSalaryVModel model)
        //{
        //    var result = new ResponseResult();

        //    var query = _salary.AsQueryable();


        //    var userName = model.FullName;
        //    query = (from salary in _dbContext.Salary
        //                join user in _dbContext.AspNetUsers on salary.UserId equals user.Id
        //    where (string.IsNullOrEmpty(model.FullName) || user.FullName.Contains(userName)) 
        //    && (!model.Month.HasValue || !model.Year.HasValue || (salary.CreatedDate.HasValue && salary.CreatedDate.Value.Month == model.Month && salary.CreatedDate.Value.Year == model.Year))
        //    && (!model.IsActive.HasValue || salary.IsActive == model.IsActive)
        //    select salary);


        //    var salaryList = await query.ToListAsync();
        //    var salaryGrouped = salaryList.GroupBy(x => x.UserId);
        //    var salaryListMapped = new List<SalaryGetAllVModel>();
        //    foreach(var group in salaryGrouped)
        //    {
        //        var user = await _userManager.FindByIdAsync(group.Key);
        //        if(user == null)
        //        {
        //            throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, $"UserId = {group.Key}"));
        //        }

        //        foreach (var item in group)
        //        {
        //            var entityMapped = _mapper.Map<Salary, SalaryGetAllVModel>(item);
        //            entityMapped.FullName = user.FullName;
        //            salaryListMapped.Add(entityMapped);
        //        }
        //    }
        //    result.Data = salaryListMapped;
        //    return result;
        //}

        public async Task Update(SalaryUpdateVModel model)
        {
            try
            {
                var entity = await _salary.FindAsync(model.Id);
                if (entity == null)
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
            if (entity == null)
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
                    entity.IsActive = !entity.IsActive;

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

        public async Task ChangeStatusMany(SalaryChangeStatusManyVModel model)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.Ids.Any())
                    {
                        foreach (var id in model.Ids)
                        {
                            var entity = await _salary.FindAsync(id);
                            if (entity == null)
                            {
                                throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, id));
                            }
                            entity.IsActive = !entity.IsActive;
                            _salary.Update(entity);
                            var result = await _dbContext.SaveChangesAsync() > 0;
                            if (!result)
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

        public Task<ResponseResult> GetIncomeInMonth(int year, int month)
        {
            var result = new ResponseResult();
            var query = _salary.AsQueryable();
            string period = $"{year}-{month.ToString("D2")}";
            var total = query.Where(x => x.IsActive && x.PayrollPeriod == period).Sum(x => (decimal?)x.TotalSalary) ?? 0;
            int bMonth = 0;
            int bYear = 0;
            if (month == 1)
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
            var months = Enumerable.Range(1, 12).Select(m => m.ToString("D2")).ToArray();
            var dbData = query
                .Where(x => x.IsActive && x.PayrollPeriod != null && x.PayrollPeriod.StartsWith(year.ToString()))
                .GroupBy(x => x.PayrollPeriod.Substring(5, 2))
                .Select(g => new {
                    month = g.Key,
                    total = g.Sum(x => (decimal?)x.TotalSalary) ?? 0
                })
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

        public async Task<ResponseResult> GetInfoForDepartmentChart()
        {
            var result = new ResponseResult();
            var departmentList = await _dbContext.Department.ToListAsync();
            var period = _salary
                .AsEnumerable()  // Chuyển về bộ nhớ client
                .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                .Select(x => x.PayrollPeriod)
                .FirstOrDefault();




            var salaryByDepartment = new Dictionary<string, decimal>();

            foreach (var department in departmentList)
            {
                var departmentName = department.Name;
                var departmentId = department.Id;
                var totalSalary = await (from salary in _dbContext.Salary
                                         join user in _dbContext.AspNetUsers on salary.UserId equals user.Id
                                         where user.DepartmentId == departmentId && salary.PayrollPeriod == period
                                         select (decimal?)salary.TotalSalary).SumAsync() ?? 0m;
                salaryByDepartment[departmentName] = totalSalary;
            }
            result.Data = salaryByDepartment;
            return result;
        }
        public async Task<ResponseResult> GetSalaryByLevel()
        {
            var result = new ResponseResult();
            try
            {
                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();
                var salaryList = await (_salary.Where(x => x.PayrollPeriod == period)).ToListAsync();

                decimal under10 = 0;
                decimal between10and20 = 0;
                decimal between20and30 = 0;
                decimal between30and40 = 0;
                decimal greaterThan40 = 0;

                foreach (var item in salaryList)
                {
                    var salary = item.TotalSalary;
                    if (salary < 10000000m)
                    {
                        under10++;
                    }
                    else if (10000000m <= salary && salary < 20000000m)
                    {
                        between10and20++;
                    }
                    else if (salary >= 20000000m && salary < 30000000m)
                    {
                        between20and30++;
                    }
                    else if (salary >= 30000000m && salary < 40000000m)
                    {
                        between30and40++;
                    }
                    else
                    {
                        greaterThan40++;
                    }
                }

                result.Data = new
                {
                    period = period,
                    under10 = under10,
                    between10and20 = between10and20,
                    between20and30 = between20and30,
                    between30and40 = between30and40,
                    greaterThan40 = greaterThan40
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }

            return result;
        }
    }
}
