using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OA.Core.Repositories;
using AngleSharp.Dom;
using OA.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace OA.Service
{
    public class EmploymentContractService : GlobalVariables, IEmploymentContractService
    {
        private readonly ApplicationDbContext _context; 
        private readonly IMapper _mapper;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IBaseRepository<SysFile> _sysFileRepo;
        private IHttpContextAccessor _contextAccessor;

        public EmploymentContractService(UserManager<AspNetUser> userManager, ApplicationDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper, IBaseRepository<SysFile> sysFileRepo):base(contextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _sysFileRepo = sysFileRepo;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResponseResult> Search(FilterEmploymentContractVModel model)
        {
            var result = new ResponseResult();
            string? keyword = model.Keyword?.ToLower();

            var recordsQuery = _context.EmploymentContract.AsQueryable();

            
            if (model.IsActive != null)
            {
                recordsQuery = recordsQuery.Where(x => x.IsActive == model.IsActive);
            }
            if (model.CreatedDate != null)
            {
                recordsQuery = recordsQuery.Where(x => x.CreatedDate.HasValue &&
                                                      x.CreatedDate.Value.Date == model.CreatedDate.Value.Date);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                recordsQuery = recordsQuery.Where(x =>
                    x.UserId.ToLower().Contains(keyword) ||
                    (x.ContractName != null && x.ContractName.ToLower().Contains(keyword)) ||
                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword))
                );
            }

            var records = model.IsDescending
                ? await recordsQuery.OrderByDescending(r => r.Id).ToListAsync()
                : await recordsQuery.OrderBy(r => r.Id).ToListAsync();

            result.Data = new Pagination()
            {
                Records = records.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList(),
                TotalRecords = records.Count()
            };

            return result;
        }


        public async Task<ResponseResult> SearchUser()
        {
            var result = new ResponseResult();

            var entity = await _context.EmploymentContract
                .FirstOrDefaultAsync(x => x.UserId == GlobalUserId);

            if (entity != null)
            {
               
                var manager = await _userManager.FindByIdAsync(entity.ManagerId.ToString());

                if (manager != null)
                {
                    var managerFullName = manager.FullName;
                    var managerEmployeeId = manager.EmployeeId;
                    var managerAvatarPath = "https://localhost:44381/avatars/aa1678f0-75b0-48d2-ae98-50871178e9bd.jfif";
        
                    if (manager.AvatarFileId.HasValue)
                    {
                        var sysFile = await _sysFileRepo.GetById((int)manager.AvatarFileId);
                        if (sysFile != null)
                        {
                            managerAvatarPath = "https://localhost:44381/" + sysFile.Path;
                        }
                    }
                   
                    result.Data = new
                    {
                        EmploymentContract = _mapper.Map<EmploymentContract>(entity),
                        Manager = new
                        {
                            ManagerFullName = managerFullName,
                            ManagerId = managerEmployeeId,
                            ManagerAvatarPath = managerAvatarPath
                        }
                    };
                }
                else
                {
                    throw new NotFoundException("Quản lý không tồn tại trong hệ thống.");
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            return result;
        }




        public async Task<ResponseResult> GetContractsExpiringSoon(FilterEmploymentContractVModel model, int daysUntilExpiration)
        {
            var result = new ResponseResult();
            string? keyword = model.Keyword?.ToLower();
            var currentDate = DateTime.UtcNow;
            var targetDate = currentDate.AddDays(daysUntilExpiration);

            var recordsQuery = _context.EmploymentContract
                .Include(ec => ec.User)
                .Where(x => x.EndDate >= currentDate && x.EndDate <= targetDate)
                .AsQueryable();

            if (model.IsActive != null)
            {
                recordsQuery = recordsQuery.Where(x => x.IsActive == model.IsActive);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                recordsQuery = recordsQuery.Where(x =>
                    x.UserId.ToLower().Contains(keyword) ||
                    (x.ContractName != null && x.ContractName.ToLower().Contains(keyword)) ||
                    (x.User != null && x.User.FullName.ToLower().Contains(keyword))
                );
            }

            var records = model.IsDescending
                ? await recordsQuery.OrderByDescending(x => x.EndDate).ToListAsync()
                : await recordsQuery.OrderBy(x => x.EndDate).ToListAsync();

            var contractList = new List<object>();
            foreach (var contract in records)
            {
                var avatarPath = contract.User.AvatarFileId != null
                    ? "https://localhost:44381/" +
                      (await _sysFileRepo.GetById((int)contract.User.AvatarFileId))?.Path
                    : null;

                contractList.Add(new
                {
                    Contract = _mapper.Map<EmploymentContractGetAllVModel>(contract),
                    User = new
                    {
                        contract.User.FullName,
                        AvatarPath = avatarPath
                    }
                });
            }

            result.Data = new Pagination()
            {
                Records = contractList.Skip((model.PageNumber - 1) * model.PageSize)
                    .Take(model.PageSize)
                    .ToList(),
                TotalRecords = records.Count
            };

            return result;
        }


        public async Task<ResponseResult> GetContractCountByType()
        {
            var result = new ResponseResult();
            var contractCounts = await _context.EmploymentContract
                .GroupBy(x => x.TypeContract)
                .Select(g => new
                {
                    TypeContract = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            result.Data = contractCounts;

            return result;
        }

        public async Task<ResponseResult> GetEmployeesStatsByYears(int year)
        {
            var result = new ResponseResult();
            try
            {
                var yearStats = new List<object>();

                for (int month = 1; month <= 12; month++)
                {
                    var previousMonth = month == 1 ? 12 : month - 1;
                    var previousYear = month == 1 ? year - 1 : year;
                    var firstDayOfMonth = new DateTime(year, month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    var firstDayOfPreviousMonth = new DateTime(previousYear, previousMonth, 1);
                    var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);
                    var contracts = await _context.EmploymentContract
                        .Where(c => c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfPreviousMonth).ToListAsync();

                    var startCount = contracts.Count(c => c.StartDate.Year == year && c.StartDate.Month == month);
                    var endCount = contracts.Count(c => c.EndDate.Year == year && c.EndDate.Month == month);
                    var contractsInMonth = contracts.Count(c =>
                        c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfMonth);

                    yearStats.Add(new
                    {
                        Month = month,
                        StartCount = startCount,                    
                        EndCount = endCount,                      
                        ContractsInMonth = contractsInMonth,       
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



        public async Task<ResponseResult> GetEmployeeStatsByMonthAndYear(int year, int month)
        {
            var result = new ResponseResult();

            var previousMonth = month == 1 ? 12 : month - 1;
            var previousYear = month == 1 ? year - 1 : year;

            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var firstDayOfPreviousMonth = new DateTime(previousYear, previousMonth, 1);
            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);
 
            var contracts = await _context.EmploymentContract
                .Where(c => c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfPreviousMonth).ToListAsync();


            var startCount = contracts.Count(c => c.StartDate.Year == year && c.StartDate.Month == month);
            var endCount = contracts.Count(c => c.EndDate.Year == year && c.EndDate.Month == month);

            var contractsInMonth = contracts.Count(c =>
                c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfMonth);

            var contractsInPreviousMonth = contracts.Count(c =>
                c.StartDate <= lastDayOfPreviousMonth && c.EndDate >= firstDayOfPreviousMonth);

            var contractsInMonthPercentChange = contractsInPreviousMonth == 0
                ? (int?)null
                : ((contractsInMonth - contractsInPreviousMonth) * 100) / contractsInPreviousMonth;

            var previousStartCount = contracts.Count(c => c.StartDate.Year == previousYear && c.StartDate.Month == previousMonth);
            var previousEndCount = contracts.Count(c => c.EndDate.Year == previousYear && c.EndDate.Month == previousMonth);

            var startPercentChange = previousStartCount == 0 ? (int?)null : ((startCount - previousStartCount) * 100) / previousStartCount;
            var endPercentChange = previousEndCount == 0 ? (int?)null : ((endCount - previousEndCount) * 100) / previousEndCount;


            var createdContractsCount = contracts.Count(c =>
            c.CreatedDate.HasValue &&
            c.CreatedDate.Value.Year == year &&
            c.CreatedDate.Value.Month == month);

            var previousCreatedContractsCount = contracts.Count(c =>
            c.CreatedDate.HasValue &&
            c.CreatedDate.Value.Year == previousYear &&
            c.CreatedDate.Value.Month == previousMonth);

            var createdContractsPercentChange = previousCreatedContractsCount == 0
            ? (int?)null
            : ((createdContractsCount - previousCreatedContractsCount) * 100) / previousCreatedContractsCount;

            result.Data = new
            {
                Year = year,
                Month = month,
                StartCount = startCount,
                StartPercentChange = startPercentChange,
                EndCount = endCount,
                EndPercentChange = endPercentChange,
                ContractsInMonth = contractsInMonth,
                ContractsInMonthPercentChange = contractsInMonthPercentChange,
                CreatedContractsCount = createdContractsCount, 
                CreatedContractsPercentChange = createdContractsPercentChange 
            };

            return result;
        }



        public async Task<ResponseResult> GetEmployeeStatsByYear(int year)
        {
            var result = new ResponseResult();


            var currentYearMonthlyCounts = new List<int>();
            var previousYearMonthlyCounts = new List<int>();


            double totalCurrentYear = 0; 
            double totalPreviousYear = 0; 

            for (int month = 1; month <= 12; month++)
            {
                var firstDayOfMonth = new DateTime(year, month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var firstDayOfPreviousYearMonth = new DateTime(year - 1, month, 1);
                var lastDayOfPreviousYearMonth = firstDayOfPreviousYearMonth.AddMonths(1).AddDays(-1);
                              
                var contractsCurrentYear = await _context.EmploymentContract
                    .Where(c =>
                        (c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfMonth)) 
                    .ToListAsync();
                             
                var contractsPreviousYear = await _context.EmploymentContract
                    .Where(c =>
                        (c.StartDate <= lastDayOfPreviousYearMonth && c.EndDate >= firstDayOfPreviousYearMonth)) 
                    .ToListAsync();

                
                var contractsInCurrentMonth = contractsCurrentYear.Count(c => c.StartDate <= lastDayOfMonth && c.EndDate >= firstDayOfMonth);
                currentYearMonthlyCounts.Add(contractsInCurrentMonth);
                totalCurrentYear += contractsInCurrentMonth;

                var contractsInPreviousMonth = contractsPreviousYear.Count(c => c.StartDate <= lastDayOfPreviousYearMonth && c.EndDate >= firstDayOfPreviousYearMonth);
                previousYearMonthlyCounts.Add(contractsInPreviousMonth);
                totalPreviousYear += contractsInPreviousMonth;
            }

            double? totalPercentChange = totalPreviousYear == 0
                ? (double?)null
                : ((totalCurrentYear - totalPreviousYear) / totalPreviousYear) * 100;  
                       
            result.Data = new
            {
                Year = year,
                MonthlyStats = currentYearMonthlyCounts.Select((count, index) => new
                {
                    Month = index + 1,
                    CurrentYearCount = count,
                    PreviousYearCount = previousYearMonthlyCounts[index]
                }).ToList(),
                TotalCurrentYear = totalCurrentYear,
                TotalPreviousYear = totalPreviousYear,
                TotalPercentChange = totalPercentChange 
            };

            return result;
        }


        public async Task<ExportStream> ExportFile(FilterEmploymentContractVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<EmploymentContractExportVModel>>(result.Data.Records);
            var exportData = ImportExportHelper<EmploymentContractExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }


        public async Task<ResponseResult> GetById(String id)
        {
            var result = new ResponseResult();
            var entity = await _context.EmploymentContract.FindAsync(id);
            if (entity != null)
            {
                result.Data = _mapper.Map<EmploymentContract>(entity);
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            return result;
        }

        
        public async Task Create(EmploymentContractCreateVModel model)
        {
            var entityCreated = _mapper.Map<EmploymentContractCreateVModel, EmploymentContract>(model);
            await _context.EmploymentContract.AddAsync(entityCreated);
            var maxId = await _context.EmploymentContract.MaxAsync(x => (string)x.Id) ?? "EC-000";
            int numberPart = int.Parse(maxId.Substring(3)) + 1; 
            entityCreated.Id = $"EC-{numberPart:D3}";
            entityCreated.CreatedDate = DateTime.Now;
            entityCreated.CreatedBy = GlobalUserName;
            var saveResult = await _context.SaveChangesAsync(); 
            if (saveResult <= 0)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "EmploymentContract"));
            }
        }


        public async Task Update(EmploymentContractUpdateVModel model)
        {
            var entity = await _context.EmploymentContract.FindAsync(model.Id);
            if (entity != null)
            {
                entity = _mapper.Map(model, entity);
                _context.EmploymentContract.Update(entity); 
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "EmploymentContract"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public async Task ChangeStatus(String id)
        {
            var entity = await _context.EmploymentContract.FindAsync(id); 
            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                _context.EmploymentContract.Update(entity); 
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "EmploymentContract"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public async Task Remove(String id)
        {
            var entity = await _context.EmploymentContract.FindAsync(id); 
            if (entity != null)
            {
                _context.EmploymentContract.Remove(entity); 
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, "EmploymentContract"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

       
    }
}
