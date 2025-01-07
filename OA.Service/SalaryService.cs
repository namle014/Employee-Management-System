using AngleSharp.Css;
using Aspose.Pdf;
using Aspose.Pdf.AI;
using Aspose.Pdf.Operators;
using AutoMapper;
using ChatGPT.Net.DTO.ChatGPTUnofficial;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
        private readonly IBaseRepository<SysFile> _sysFileRepo;

        public SalaryService(ApplicationDbContext dbContext, IMapper mapper, IBaseRepository<SysFile> sysFileRepo, IHttpContextAccessor contextAccessor, UserManager<AspNetUser> userManager) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _salary = dbContext.Set<Salary>();
            _employments = dbContext.Set<EmploymentContract>();
            _mapper = mapper;
            _userManager = userManager;
            _sysFileRepo = sysFileRepo;
        }
        #region --ADMIN--
        public async Task Create()
        {
            var today = DateTime.Now;
            var year = today.Year;
            var month = today.Month;

            bool isEndOfMonth = today.Day == DateTime.DaysInMonth(year, month);

            if (!isEndOfMonth)
            {
                throw new BadRequestException("Hôm nay không phải là ngày cuối tháng, không thể tạo chu kỳ lương.");
            }
            var period = $"{month.ToString("D2")}-{year}";

            bool periodExists = await _salary.AnyAsync(x => x.IsActive && x.PayrollPeriod == period);
            if (periodExists)
            {
                throw new BadRequestException($"Đã tồn tại chu kỳ lương {period}.");
            }

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


                var totalReward = _dbContext.Reward.Where(x => x.UserId == userId && x.IsActive &&x.Date.Month == month && x.Date.Year == year).Sum(x => x.Money) ?? 0;
                var totalDiscipline = _dbContext.Discipline.Where(x => x.UserId == userId && x.IsActive && x.Date.Month == month && x.Date.Year == year).Sum(x => x.Money);
                var totalBenefit = await (from bUser in _dbContext.BenefitUser
                                          join benefit in _dbContext.Benefit on bUser.BenefitId equals benefit.Id
                                          where (bUser.UserId == userId && benefit.IsActive)
                                          select bUser.BenefitContribution).SumAsync();

                var insuranceList = await (from insuranceUser in _dbContext.InsuranceUser
                                     join insurance in _dbContext.Insurance on insuranceUser.InsuranceId equals insurance.Id
                                     where (insuranceUser.UserId == userId && insurance.IsActive && insuranceUser.Status != 2)
                                     select insuranceUser).ToListAsync();

                var bhtn = Convert.ToDouble(_dbContext.SysConfigurations.FirstOrDefault(x => x.Key == "MD_BHTN")?.Value);
                double totalInsurance = 0;
                double maxBaseSalary = 20 * 2340000;
                double maxBasicSalary = 20 * bhtn;

                foreach (var ins in insuranceList)
                {
                    var rate = ins.EmployeeContributionRate;
                    var status = (ins.InsuranceId == "BHYT") ? 2 : 1;
                    var maxSalary = (ins.InsuranceId == "BHTN") ? maxBasicSalary : maxBaseSalary;
                    var PaidInsuranceContribution = Math.Min(dailyWage, maxSalary) * (double)rate;

                    var eIns = await _dbContext.InsuranceUser.FirstOrDefaultAsync(x => x.InsuranceId == ins.InsuranceId);
                    if (eIns != null)
                    {
                        
                        eIns.Status = status;
                    }

                    totalInsurance += PaidInsuranceContribution;
                }

                _dbContext.InsuranceUser.UpdateRange(insuranceList);
                await _dbContext.SaveChangesAsync();

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
                entity.NumberOfWorkingHours = totalHours;
                entity.TotalInsurance = (decimal)totalInsurance;
                entity.TotalBenefit = totalBenefit;
                entity.TotalReward = totalReward;
                entity.TotalDiscipline = totalDiscipline;
                entity.PITax = (decimal)PITax;
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = GlobalUserName;
                entity.IsActive = true;
                entity.PayrollPeriod = period;
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
                    entityMapped.Reward = item.TotalReward;
                    entityMapped.Discipline = item.TotalDiscipline;
                    
                    entityMapped.Timekeeping = item.NumberOfWorkingHours;

                    entityMapped.Benefit = item.TotalBenefit;
                    entityMapped.Insurance = item.TotalInsurance;

                    entityMapped.PITax = item.PITax;
                    entityMapped.TotalSalary = item.TotalSalary;

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

                var salary = _mapper.Map<Salary, SalaryGetByIdVModel>(entity);
                var user = await _userManager.FindByIdAsync(salary.UserId);
                salary.EmployeeId = user != null ? user.EmployeeId : null;
                salary.DepartmentName = user != null ? await _dbContext.Department.Where(x => x.IsActive && x.Id == (user.DepartmentId ?? 1))?.Select(x => x.Name).FirstOrDefaultAsync() : string.Empty;
                salary.RoleName = user != null ? (await _userManager.GetRolesAsync(user)).ToList() : new List<string>();
                salary.Date = entity.Date.ToString("dd-MM-yyyy");
                salary.FullName = user != null ? (user.FullName != null ? user.FullName : string.Empty) : string.Empty;
                salary.AvatarPath = user.AvatarFileId != null ? "https://localhost:44381/" + (await _sysFileRepo.GetById((int)user.AvatarFileId))?.Path : null;
                result.Data = salary;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }

        public async Task Update(string Id)
        {
            try
            {
                var entity = await _salary.FindAsync(Id);
                
                if (entity == null)
                {
                    throw new NotFoundException("Không tìm thấy dữ liệu");
                }
                var userId = entity.UserId;
                var month = Convert.ToInt32(entity.PayrollPeriod.Substring(0, 2));
                var year = Convert.ToInt32(entity.PayrollPeriod.Substring(3, 4));
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

                double totalHours = 12;

                var workingHours = workingDays * 8;

                var dailyWage = (double)totalBasicSalary / workingHours * totalHours;

                entity.ProRatedSalary = (decimal)dailyWage;
                entity.NumberOfWorkingHours = totalHours;

                var totalBenefit = entity.TotalBenefit;
                var totalReward = entity.TotalReward ?? 0m;
                var totalInsurance = entity.TotalInsurance;
                var totalDiscipline = entity.TotalDiscipline ?? 0m;

                var relative = (await _userManager.FindByIdAsync(userId))?.EmployeeDependents;

                var taxableIncome = (dailyWage) > 11000000 ? ((decimal)dailyWage + totalReward - totalDiscipline + totalBenefit - 4400000 * relative - (decimal)totalInsurance - 11000000) : 0;
                double PITax = getPITax(taxableIncome);

                entity.PITax = (decimal)PITax;
                entity.TotalSalary = (decimal)dailyWage + totalReward - totalDiscipline + totalBenefit - totalInsurance - (decimal)PITax;
                entity.SalaryPayment = (decimal)dailyWage + totalReward - totalDiscipline + totalBenefit;
                entity.IsActive = true;
                entity.IsPaid = false;
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

        public async Task PaymentConfirmation(string Id)
         {
            try
                {
                var entity = await _salary.FindAsync(Id);
                if (entity != null)
                {
                    entity.UpdatedDate = DateTime.Now;
                    entity.UpdatedBy = GlobalUserName;
                    entity.IsPaid = true;

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
        public async Task Remove(string id)
        {
            var entity = await _salary.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            entity.IsActive = false;
            entity.UpdatedBy = GlobalUserName;
            entity.UpdatedDate = DateTime.Now;
            _salary.Update(entity);

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
                            entity.UpdatedDate = DateTime.Now;
                            entity.UpdatedBy = GlobalUserName;
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

        public async Task<ResponseResult> GetYearIncome(int year)
        {
            var result = new ResponseResult();
            var query = _salary.AsQueryable();
            var months = Enumerable.Range(1, 12).Select(m => m.ToString("D2")).ToArray();
            var dbData = (await _salary
                .Where(x => x.IsActive && x.PayrollPeriod.Contains(year.ToString()))
                .ToListAsync()) // Tải dữ liệu về client
                .OrderBy(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", null))
                .GroupBy(x => x.PayrollPeriod)
                .Select(g => g.Sum(x => x.TotalSalary)) 
                .ToList();



            var bYear = year - 1;

            var bdbData = (await _salary
                    .Where(x => x.IsActive && x.PayrollPeriod.Contains(bYear.ToString()))
                    .ToListAsync()) // Tải dữ liệu về client
                    .OrderBy(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", null))
                    .GroupBy(x => x.PayrollPeriod)
                    .Select(g => g.Sum(x => x.TotalSalary))
                    .ToList();

            result.Data = new
            {
                yearList = dbData,
                bYearList = bdbData
            };

            return result;
        }

        public async Task<ResponseResult> GetInfoForDepartmentChart()
        {
            var result = new ResponseResult();
            var departmentList = await _dbContext.Department.Where(x => x.IsActive).ToListAsync();
            var period = _salary
                .AsEnumerable()  // Chuyển về bộ nhớ client
                .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                .Select(x => x.PayrollPeriod)
                .FirstOrDefault();

            if (period != null)
            {
                var salaryByDepartment = new Dictionary<string, decimal>();

                foreach (var department in departmentList)
                {
                    var departmentName = department.Name;
                    var departmentId = department.Id;
                    var totalSalary = await (from salary in _dbContext.Salary
                                             join user in _dbContext.AspNetUsers on salary.UserId equals user.Id
                                             where user.DepartmentId == departmentId && salary.PayrollPeriod == period && salary.IsActive == true
                                             select (decimal?)salary.TotalSalary).SumAsync() ?? 0m;
                    salaryByDepartment[departmentName] = totalSalary;
                }
                result.Data = salaryByDepartment;
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
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
                if (period != null)
                {
                    var salaryList = await (_salary.Where(x => x.PayrollPeriod == period && x.IsActive)).ToListAsync();

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
                else
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }

            return result;
        }
        public async Task<ResponseResult> GetInfoForSalarySummary()
        {
            var result = new ResponseResult();
            try
            {
                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();

                if (period != null)
                {
                    var salaryList = await (_salary.Where(x => x.PayrollPeriod == period && x.IsActive)).ToListAsync();
                    decimal total = 0;
                    decimal pitax = 0;
                    decimal totalInsurance = 0;
                    foreach (var item in salaryList)
                    {
                        total += item.TotalSalary;
                        pitax += item.PITax;
                        totalInsurance += item.TotalInsurance;

                    }
                    result.Data = new
                    {
                        total = Math.Round(total / 1000000m, 2),
                        PITax = Math.Round(pitax / 1000000m, 2),
                        totalInsurance = Math.Round(totalInsurance / 1000000m, 2),
                    };
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
            return result;
        }
        public async Task<ResponseResult> GetTotalIncomeOverTime()
        {
            var result = new ResponseResult();
            try
            {
                var payrollPeriod = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();
                if (payrollPeriod != null)
                {
                    var month = Convert.ToInt32(payrollPeriod.Substring(0, 2));
                    var year = Convert.ToInt32(payrollPeriod.Substring(3, 4));

                    var period = string.Empty;
                    if (month <= 1) {
                        year--;
                    }
                    period = year.ToString();

                    var salaryList = await _salary
                        .Where(x => x.PayrollPeriod.Contains(period) && x.IsActive)
                        .ToListAsync();

                    var incomeList = salaryList
                        .GroupBy(x => x.PayrollPeriod)
                        .Select(group => new TotalIncomeVmodel
                        {
                            payrollPeriod = group.Key,
                            TotalIncome = group.Sum(x => x.SalaryPayment),
                            TotalSalary = group.Sum(x => x.TotalSalary)
                        })
                        .ToList();

                    result.Data = incomeList;
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
            return result;
        }
        public async Task<ResponseResult> GetIncomeStructure()
        {
            var result = new ResponseResult();
            try
            {
                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();

                if(period != null)
                {
                    var month = Convert.ToInt32(period.Substring(0, 2));
                    var year = Convert.ToInt32(period.Substring(3, 4));
                    var baseSalary = await _salary.Where(x => x.PayrollPeriod == period && x.IsActive).Select(x => x.ProRatedSalary).SumAsync();
                    var empReward = await _dbContext.Reward.Where(x => x.IsActive && x.IsReceived && x.Date.Month == month && x.Date.Year == year  ).Select(x => x.Money ?? 0).SumAsync();
                    var PITax = await _salary.Where(x => x.PayrollPeriod == period && x.IsActive).Select(x => x.PITax).SumAsync();
                    var birthday = await (from bUser in _dbContext.BenefitUser
                                               join benefit in _dbContext.Benefit on bUser.BenefitId equals benefit.Id
                                               where ( benefit.IsActive && benefit.Name.ToLower().Contains("sinh nhật"))
                                               select bUser.BenefitContribution).SumAsync();
                    var total = baseSalary + empReward + PITax + birthday;
                    result.Data = new
                    {
                        baseSalary = total != 0m ? Math.Round(baseSalary / total * 100, 2) : 0,
                        reward = total != 0m ? Math.Round(empReward / total * 100, 2) : 0,
                        PITax = total != 0m ? Math.Round(PITax / total * 100, 2) : 0,
                        birthday = total != 0m ? Math.Round(birthday / total * 100, 2) : 0
                    };
                }
                else
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }
            }
            catch (Exception ex) {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult> GetPeriod()
        {
            var result = new ResponseResult();
            try
            {
                var periodList = await _salary
                    .Where(x => x.IsActive)
                    .Select(x => x.PayrollPeriod)
                    .Distinct()
                    .ToListAsync();

                periodList = periodList
                    .OrderBy(x => DateTime.ParseExact(x, "MM-yyyy", null))
                    .ToList();

                result.Data = periodList;
            }
            catch (Exception ex)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            return result;
        }
        public async Task<ResponseResult> GetTotalBySex()
        {
            var result = new ResponseResult();
            try
            {
                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();

                if(period != null)
                {
                    var male = await (from user in _dbContext.AspNetUsers
                                      join salary in _salary on user.Id equals salary.UserId
                                      where (user.Gender == true && salary.IsActive && salary.PayrollPeriod == period)
                                      select (salary.SalaryPayment)).SumAsync();

                    var female = await (from user in _dbContext.AspNetUsers
                                        join salary in _salary on user.Id equals salary.UserId
                                        where (user.Gender == false && salary.IsActive && salary.PayrollPeriod == period)
                                        select (salary.SalaryPayment)).SumAsync();

                    var other = await (from user in _dbContext.AspNetUsers
                                       join salary in _salary on user.Id equals salary.UserId
                                       where (user.Gender == null && salary.IsActive && salary.PayrollPeriod == period)
                                       select (salary.SalaryPayment)).SumAsync();

                    var total = male + female + other;

                    result.Data = new
                    {
                        male = Math.Round(male / total * 100, 2),
                        female = Math.Round(female / total * 100, 2),
                        other = Math.Round(other / total * 100, 2),
                    };
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
            return result;
        }
        public async Task<ResponseResult> GetGrossTotal()
        {
            var result = new ResponseResult();
            try
            {

                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();

                if (period != null)
                {
                    var netSalary = (double)await (_salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.TotalSalary)).SumAsync();
                    var PITax = (double)await (_salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.PITax)).SumAsync();
                    var totalInsurance = (double) await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.TotalInsurance).SumAsync();

                    var total = netSalary + PITax + totalInsurance;

                    result.Data = new
                    {
                        netSalaries = netSalary,
                        PITaxes = PITax,
                        ins = totalInsurance,
                        netSalary = total > 0 ? Math.Round(netSalary / total * 100, 2) : 0,
                        PITax = total > 0 ? Math.Round(PITax / total * 100, 2) : 0,
                        totalInsurance = total > 0 ? Math.Round(totalInsurance / total * 100, 2) : 0,
                    };
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
            return result;
        }
        public async Task<ResponseResult> GetGrossTotalByDepartments()
        {
            var result = new ResponseResult();
            try
            {
                var departmentList = await _dbContext.Department.Where(x => x.IsActive).ToListAsync();
                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();

                if (period != null)
                {
                    var salaryByDepartment = new Dictionary<string, decimal>();
                    var salaryPercent = new Dictionary<string, double>();
                    var total = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.SalaryPayment).SumAsync();

                    foreach (var department in departmentList)
                    {
                        var departmentName = department.Name;
                        var departmentId = department.Id;
                        var totalSalary = await (from salary in _dbContext.Salary
                                                 join user in _dbContext.AspNetUsers on salary.UserId equals user.Id
                                                 where user.DepartmentId == departmentId && salary.PayrollPeriod == period && salary.IsActive == true
                                                 select (decimal?)salary.SalaryPayment).SumAsync() ?? 0m;
                        salaryByDepartment[departmentName] = totalSalary;
                        salaryPercent[departmentName] = Math.Round(((double)(totalSalary / total)) * 100, 2);
                    }
                    result.Data = new
                    {
                        period = period,                                                                                                                                                                                                                                                                                                                            
                        salaryByDepartment = salaryByDepartment,
                        salaryPercent = salaryPercent
                    };
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
            return result;
        }
        public async Task<ResponseResult> GetTotalMaxMin()
        {
            var result = new ResponseResult();
            try
            {

                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client                                                                                                                                                                     
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();

                if (period != null)
                {
                    var maxSalaryRow = await _salary
                        .Where(x => x.IsActive && x.PayrollPeriod == period)
                        .OrderByDescending(x => x.SalaryPayment) // Sắp xếp giảm dần theo SalaryPayment
                        .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên

                    var minSalaryRow = await _salary
                        .Where(x => x.IsActive && x.PayrollPeriod == period)
                        .OrderBy(x => x.SalaryPayment) // Sắp xếp tăng dần theo SalaryPayment
                        .FirstOrDefaultAsync();

                    var maxUser = maxSalaryRow != null
                        ? (await _userManager.FindByIdAsync(maxSalaryRow.UserId))?.FullName
                        : "Dữ liệu không tồn tại";

                    var minUser = minSalaryRow != null
                        ? (await _userManager.FindByIdAsync(minSalaryRow.UserId))?.FullName
                        : "Dữ liệu không tồn tại";

                    var max = maxSalaryRow != null ? maxSalaryRow.SalaryPayment : 0;
                    var min = minSalaryRow != null ? minSalaryRow.SalaryPayment : 0;

                    result.Data = new
                    {
                        maxUser = maxUser,
                        max = max,
                        minUser = minUser,
                        min = min
                    };
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
            return result;
        }
        public async Task<ResponseResult> GetDisplayInfo()
        {
            var result = new ResponseResult();
            try
            {

                var period = _salary
                    .AsEnumerable()  // Chuyển về bộ nhớ client
                    .OrderByDescending(x => DateTime.ParseExact(x.PayrollPeriod, "MM-yyyy", CultureInfo.InvariantCulture))
                    .Select(x => x.PayrollPeriod)
                    .FirstOrDefault();

                if (period != null)
                {
                    var year = Convert.ToInt32(period.Substring(3, 4));
                    var month = Convert.ToInt32(period.Substring(0, 2));
                    if(month == 1)
                    {
                        month = 12;
                        year--;
                    }
                    else { month--; }
                    var bPeriod = $"{month}-{year}";

                    var grossTotal = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.SalaryPayment).SumAsync();
                    var netTotal = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.TotalSalary).SumAsync();
                    var basicTotal = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.ProRatedSalary).SumAsync();

                    var userCount = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).CountAsync();

                    var grossProgress = grossTotal > 0m ? (decimal?)Math.Round((netTotal / grossTotal) * 100, 2) : null;
                    var netProgress = netTotal > 0m && userCount > 0 ? (decimal?)Math.Round((netTotal / userCount) / netTotal *100 , 2) : null;
                    var netPerPerson = userCount > 0 ? (decimal?)Math.Round(netTotal / userCount, 2) : null;
                    var basicProgress = userCount > 0 && basicTotal > 0 ? (decimal?)Math.Round((basicTotal / userCount) / basicTotal * 100, 2) : null;
                    var basicPerPerson = userCount > 0 ? (decimal?)Math.Round(basicTotal / userCount, 2) : null;

                    bool bPeriodExists = await _salary.AnyAsync(x => x.IsActive && x.PayrollPeriod == bPeriod);

                    float? grossPercent = 0, netPercent = 0, basicPercent = 0;
                    decimal bGross = 0, bNet = 0, bBasic = 0;
                    if (bPeriodExists)
                    {
                         bGross = await _salary.Where(x => x.IsActive && x.PayrollPeriod == bPeriod).Select(x => x.SalaryPayment).SumAsync();
                         bNet = await _salary.Where(x => x.IsActive && x.PayrollPeriod == bPeriod).Select(x => x.TotalSalary).SumAsync();
                         bBasic = await _salary.Where(x => x.IsActive && x.PayrollPeriod == bPeriod).Select(x => x.ProRatedSalary).SumAsync();

                        grossPercent = bGross != 0 ? (float)(((grossTotal - bGross) / bGross) * 100) : null ;
                        netPercent = bNet !=0 ? (float)(((netTotal - bNet) / bNet) * 100) : null;
                        basicPercent = bBasic != 0 ? (float)(((basicTotal - bBasic) / bBasic) * 100) : null;
                    }

                    result.Data = new
                    {
                        grossTotal = grossTotal,
                        bGross = (grossTotal - bGross),
                        grossPercent = grossPercent,
                        grossProgress = grossProgress,
                        netTotal = netTotal,
                        bTotal = (netTotal - bNet),
                        netPercent = netPercent,
                        netProgress = netProgress,
                        netPerPerson = netPerPerson,
                        basicTotal = basicTotal,
                        bBasic = (basicTotal - bBasic),
                        basicPercent = basicPercent,
                        basicProgress = basicProgress,
                        basicPerPerson = basicPerPerson
                    };
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
            return result;
        }
        public async Task<ResponseResult> GetPayrollOfDepartmentOvertime(int year)
        {
            var result = new ResponseResult();
            try
            {
                var departmentList = await _dbContext.Department.Where(x => x.IsActive).ToListAsync();
                var salaryByDepartment = new Dictionary<string, List<KeyValuePair<string, decimal>>>();

                foreach (var department in departmentList)
                {
                    var departmentName = department.Name;
                    var departmentId = department.Id;

                    var totalSalariesByPeriod = await (from salary in _dbContext.Salary
                                                       join user in _dbContext.AspNetUsers on salary.UserId equals user.Id
                                                       where user.DepartmentId == departmentId
                                                             && salary.PayrollPeriod.Contains(year.ToString())
                                                             && salary.IsActive == true
                                                       group salary by salary.PayrollPeriod into grouped
                                                       select new
                                                       {
                                                           PayrollPeriod = grouped.Key,
                                                           TotalSalary = grouped.Sum(s => (decimal?)s.SalaryPayment) ?? 0m
                                                       }).ToListAsync();

                    if (!totalSalariesByPeriod.Any())
                    {
                        salaryByDepartment[departmentName] = new List<KeyValuePair<string, decimal>>
                            {
                                new KeyValuePair<string, decimal>("No Data", 0m)
                            };
                    }
                    else
                    {
                        salaryByDepartment[departmentName] = totalSalariesByPeriod
                            .Select(x => new KeyValuePair<string, decimal>(x.PayrollPeriod, x.TotalSalary))
                            .ToList();
                    }

                    result.Data = salaryByDepartment;
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult> GetPayrollReport(int year)
        {
            var result = new ResponseResult();
            try
            {
                var totalInsurance = await (from salary in _salary
                                            join insUser in _dbContext.InsuranceUser on salary.UserId equals insUser.UserId
                                            where salary.IsActive && salary.PayrollPeriod.Contains(year.ToString())
                                            group salary by salary.PayrollPeriod into grouped
                                            select new
                                            {
                                                PayrollPeriod = grouped.Key,
                                                TotalInsurance = grouped.Sum(s => (decimal?)s.TotalInsurance) ?? 0m
                                            })
                                            .OrderBy(x => x.PayrollPeriod).ToListAsync();
                var PITaxByPayroll = await _salary
                    .Where(x => x.IsActive && x.PayrollPeriod.Contains(year.ToString()))
                    .GroupBy(x => x.PayrollPeriod)
                    .Select(group => new
                    {
                        PayrollPeriod = group.Key,
                        TotalPITax = group.Sum(salary => salary.PITax)
                    })
                    .ToListAsync();

                var netTotal = await _salary
                    .Where(x => x.IsActive && x.PayrollPeriod.Contains(year.ToString()))
                    .GroupBy(x => x.PayrollPeriod)
                    .Select(group => new
                    {
                        PayrollPeriod = group.Key,
                        TotalSalary = group.Sum(salary => salary.TotalSalary)
                    })
                    .ToListAsync();

                var grossTotal = await _salary
                    .Where(x => x.IsActive && x.PayrollPeriod.Contains(year.ToString()))
                    .GroupBy(x => x.PayrollPeriod)
                    .Select(group => new
                    {
                        PayrollPeriod = group.Key,
                        SalaryPayment = group.Sum(salary => salary.SalaryPayment)
                    })
                    .ToListAsync();

                result.Data = new
                {
                    insurance = totalInsurance,
                    PITax = PITaxByPayroll,
                    net = netTotal,
                    gross = grossTotal
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult> GetUnpaidSalary(SalaryFilterVModel model, int year)
        {
            var result = new ResponseResult();
            try
            {
                var unpaidList = await _salary.Where(x => x.IsActive && x.IsPaid == false && x.Date.Year == year).ToListAsync();


                var salaryGrouped = unpaidList.GroupBy(x => x.UserId);
                var salaryListMapped = new List<UnPaidSalaryVModel>();
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
                        var entityMapped = _mapper.Map<Salary, UnPaidSalaryVModel>(item);
                        entityMapped.BasicSalary = item.ProRatedSalary;
                        entityMapped.Reward = item.TotalReward;
                        entityMapped.Discipline = item.TotalDiscipline;

                        entityMapped.Timekeeping = item.NumberOfWorkingHours;

                        entityMapped.Benefit = item.TotalBenefit;
                        entityMapped.Insurance = item.TotalInsurance;

                        entityMapped.PITax = item.PITax;

                        entityMapped.FullName = user.FullName;
                        entityMapped.TotalSalary = item.TotalSalary;
                        entityMapped.AvatarPath = user.AvatarFileId != null ? "https://localhost:44381/" + (await _sysFileRepo.GetById((int)user.AvatarFileId))?.Path : null;
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

            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult> PayrollOverview(string period)
        {
            var result = new ResponseResult();
            try
            {
                var month = Convert.ToInt32(period.Substring(0, 2));
                var year = Convert.ToInt32(period.Substring(3, 4));

                var previousMonth = month == 1 ? 12 : month - 1;
                var previousYear = month == 1 ? year - 1 : year;

                var firstDayOfMonth = new DateTime(year, month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var firstDayOfPreviousMonth = new DateTime(previousYear, previousMonth, 1);
                var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);

                var grossTotal = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.SalaryPayment).SumAsync();
                var netTotal = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.TotalSalary).SumAsync();
                
                var contracts = await _dbContext.EmploymentContract
                .Where(c => c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfPreviousMonth).ToListAsync();
                
                var contractsInMonth = contracts.Count(c =>
                c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfMonth);

                var totalBenefit = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.TotalBenefit).SumAsync();

                var totalPITax = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.PITax).SumAsync();

                var totalInsurance = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.TotalInsurance).SumAsync();

                var avgGross = contractsInMonth !=0 ? Math.Round(grossTotal / contractsInMonth, 2) : 0;

                var avgNet = contractsInMonth !=0 ? Math.Round(netTotal / contractsInMonth, 2) : 0;

                var avgInsurance = contractsInMonth !=0 ? Math.Round(totalInsurance / contractsInMonth, 2) : 0;

                var avgTax = contractsInMonth != 0 ? Math.Round(totalPITax / contractsInMonth, 2) : 0;

                var departmentList = await _dbContext.Department.Where(x => x.IsActive).ToListAsync();

                var salaryPercent = new Dictionary<string, double>();
                var salaryByDepartment = new Dictionary<string, decimal>();

                var total = await _salary.Where(x => x.IsActive && x.PayrollPeriod == period).Select(x => x.SalaryPayment).SumAsync();

                foreach (var department in departmentList)
                {
                    var departmentName = department.Name;
                    var departmentId = department.Id;
                    var totalSalary = await (from salary in _dbContext.Salary
                                             join user in _dbContext.AspNetUsers on salary.UserId equals user.Id
                                             where user.DepartmentId == departmentId && salary.PayrollPeriod == period && salary.IsActive == true
                                             select (decimal?)salary.SalaryPayment).SumAsync() ?? 0m;
                    salaryByDepartment[departmentName] = totalSalary;
                    salaryPercent[departmentName] = total != 0m ? Math.Round(((double)(totalSalary / total)) * 100, 2) : 0.0;
                }

                result.Data = new
                {
                    grossTotal = grossTotal,
                    netTotal = netTotal,
                    totalPersonnel = contractsInMonth,
                    totalBenefit = totalBenefit,
                    totalPITax = totalPITax,
                    totalInsurance = totalInsurance,
                    avgGross = avgGross,
                    avgNet = avgNet,
                    avgInsurance = avgInsurance,
                    avgTax = avgTax,
                    salaryByDepartment = salaryByDepartment,
                    salaryPercent = salaryPercent
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        #endregion --ADMIN--
        #region --USER--
        public async Task<ResponseResult> GetMeSalaryInfo(string id)
        {
            var result = new ResponseResult();
            var userId = GlobalUserId;
            userId = userId != null ? userId : string.Empty;
            try
            {
                var entity = await _salary.FirstOrDefaultAsync(s => s.Id == id);
                if (entity == null)
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }

                var salary = _mapper.Map<Salary, SalaryGetByIdVModel>(entity);
                var user = await _userManager.FindByIdAsync(userId);
                salary.EmployeeId = user != null ? user.EmployeeId : null;
                salary.DepartmentName = user != null ? await _dbContext.Department.Where(x => x.IsActive && x.Id == (user.DepartmentId ?? 1))?.Select(x => x.Name).FirstOrDefaultAsync() : string.Empty;
                salary.RoleName = user != null ? (await _userManager.GetRolesAsync(user)).ToList() : new List<string>();
                salary.Date = entity.Date.ToString("dd-MM-yyyy");
                salary.FullName = user != null ? (user.FullName != null ? user.FullName : string.Empty) : string.Empty;
                salary.AvatarPath = user.AvatarFileId != null ? "https://localhost:44381/" + (await _sysFileRepo.GetById((int)user.AvatarFileId))?.Path : null;
                result.Data = salary;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult>GetIncomeByYear(int year)
        {
            var result = new ResponseResult();
            try
            {
                var userId = GlobalUserId != null ? GlobalUserId : string.Empty;
                var mySalaryList = await _salary.Where(x => x.IsActive && x.PayrollPeriod.Contains(year.ToString()) && x.UserId == userId).OrderBy(x => x.PayrollPeriod).Select(x => x.TotalSalary).ToListAsync();
                result.Data = mySalaryList;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult> GetMeInfoCycle(int year)
        {
            var result = new ResponseResult();
            try
            {
                var infoCycleList = new List<MyInfoCycleVModel>();
                var userId = GlobalUserId != null ? GlobalUserId : string.Empty;
                var mySalaryList = await _salary.Where(x => x.IsActive && x.UserId == userId && x.PayrollPeriod.Contains(year.ToString())).OrderBy(x => x.PayrollPeriod).ToListAsync();
                foreach(var salary in mySalaryList)
                {
                    var entity = new MyInfoCycleVModel();
                    entity.Id = salary.Id;
                    entity.Period = salary.PayrollPeriod;
                    entity.NumberOfWorkingHours = salary.NumberOfWorkingHours;
                    entity.Ispaid = salary.IsPaid;
                    entity.TotalSalary = salary.TotalSalary;
                    infoCycleList.Add(entity);
                }
                result.Data = infoCycleList;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        public async Task<ResponseResult> GetMeInfo()
        {
            var result = new ResponseResult();
            try
            {
                var userId = GlobalUserId != null ? GlobalUserId : string.Empty;
                var entity = new MyInfo();
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    entity.FullName = user != null ? (user.FullName != null ? user.FullName : string.Empty) : string.Empty;
                    entity.Address = user != null ? (user.Address != null ? user.Address : string.Empty) : string.Empty;
                    entity.AvatarPath = user.AvatarFileId != null ? "https://localhost:44381/" + (await _sysFileRepo.GetById((int)user.AvatarFileId))?.Path : null;
                    var date = await _dbContext.EmploymentContract.Where(x => x.UserId == userId && x.IsActive).Select(x => x.StartDate).FirstOrDefaultAsync();
                    entity.StartDateWork = date.ToString("dd-MM-yyyy");

                    entity.PayrollCycle = await _salary.Where(x => x.IsActive && x.UserId == userId).CountAsync();
                    entity.RoleName = user != null ? (await _userManager.GetRolesAsync(user)).ToList() : new List<string>();
                    entity.DepartmentName = user != null ? await _dbContext.Department.Where(x => x.IsActive && x.Id == (user.DepartmentId ?? 1))?.Select(x => x.Name).FirstOrDefaultAsync() : string.Empty;
                    entity.PhoneNumber = user != null ? (user.PhoneNumber != null ? user.PhoneNumber : string.Empty) : string.Empty;
                    entity.Email = user != null ? (user.Email != null ? user.Email : string.Empty) : string.Empty;
                }
                result.Data = entity;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
        #endregion --USER--
    }
}