using AngleSharp.Dom;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;

namespace OA.Service
{
    public class TimeOffService : ITimeOffService
    {
        private readonly ApplicationDbContext _context; 
        private readonly IMapper _mapper;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly IBaseRepository<SysFile> _sysFileRepo;
        private readonly IBaseRepository<Department> _departmentService;

        public TimeOffService(IBaseRepository<Department> departmentService, IBaseRepository<SysFile> sysFileRepo, RoleManager<AspNetRole> roleManager, UserManager<AspNetUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _sysFileRepo = sysFileRepo;
            _departmentService = departmentService;
        }

        public async Task<ResponseResult> Search(FilterTimeOffVModel model)
        {
            var result = new ResponseResult();
            string? keyword = model.Keyword?.ToLower();

            var recordsQuery = _context.TimeOff.AsQueryable();

            
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
                    (x.Reason != null && x.Reason.ToLower().Contains(keyword)) ||
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


        public async Task<ResponseResult> CountTimeOffsInMonth(int year, int month)
        {
            var result = new ResponseResult();

            if (month < 1 || month > 12)
            {
                throw new ArgumentException("Tháng phải nằm trong khoảng từ 1 đến 12.");
            }
                       
            var currentMonthCount = await _context.TimeOff
                .Where(x => x.StartDate.Year == year && x.StartDate.Month == month)
                .CountAsync();
                      
            var previousMonth = month == 1 ? 12 : month - 1;
            var previousYear = month == 1 ? year - 1 : year;
            var previousMonthCount = await _context.TimeOff
                .Where(x => x.StartDate.Year == previousYear && x.StartDate.Month == previousMonth)
                .CountAsync();

                      double? percentageIncrease = null;
            if (previousMonthCount > 0)
            {
                percentageIncrease = ((double)(currentMonthCount - previousMonthCount) / previousMonthCount) * 100;
            }
                      
            result.Data = new
            {
                Year = year,
                Month = month,
                CurrentMonthCount = currentMonthCount,
                PreviousMonthCount = previousMonthCount,
                PercentageIncrease = percentageIncrease
            };

            return result;
        }


        public async Task<ResponseResult> GetPendingFutureTimeOffs(DateTime fromDate)
        {
            var result = new ResponseResult();

            try
            {
                var timeOffRecords = await _context.TimeOff
                    .Where(x => x.StartDate >= fromDate && !x.IsAccepted)
                    .ToListAsync();

              
                var timeOffDetails = new List<dynamic>();

                foreach (var timeOff in timeOffRecords)
                {
                    var user = await _userManager.FindByIdAsync(timeOff.UserId);
                    if (user != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);

                        var userAvatarPath = (string)null;
                        if (user.AvatarFileId.HasValue)
                        {                 
                            var sysFile = await _sysFileRepo.GetById((int)user.AvatarFileId);     
                            if (sysFile != null)
                            {
                                userAvatarPath = "https://localhost:44381/" + sysFile.Path;
                            }
                        }

                        var userDepartment = user.DepartmentId.HasValue ? 
                            (await _departmentService.GetById(user.DepartmentId.Value))?.Name ?? "": "";

                        dynamic timeOffModel = new
                        {
                            Id = timeOff.Id,
                            StartDate = timeOff.StartDate,
                            EndDate = timeOff.EndDate,
                            IsAccepted = timeOff.IsAccepted,
                            UserFullName = user.FullName,
                            UserRoles = userRoles.ToList(),
                            UserEmployeeId = user.EmployeeId,
                            UserAvatarPath = userAvatarPath,
                            UserDepartment = userDepartment
                        };
                        timeOffDetails.Add(timeOffModel);
                    }
                }

                result.Data = timeOffDetails;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }

            return result;
        }




        public async Task<ExportStream> ExportFile(FilterTimeOffVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<TimeOffExportVModel>>(result.Data.Records);
            var exportData = ImportExportHelper<TimeOffExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }


        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();
            var entity = await _context.TimeOff.FindAsync(id);
            if (entity != null)
            {
                result.Data = _mapper.Map<TimeOff>(entity);
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            return result;
        }

        
        public async Task Create(TimeOffCreateVModel model)
        {
            var entityCreated = _mapper.Map<TimeOffCreateVModel, TimeOff>(model);
            await _context.TimeOff.AddAsync(entityCreated);
            var maxId = await _context.TimeOff.MaxAsync(x => (int?)x.Id) ?? 0; 
            entityCreated.Id = maxId + 1;
            var saveResult = await _context.SaveChangesAsync(); 
            if (saveResult <= 0)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "TimeOff"));
            }
        }


        public async Task Update(TimeOffUpdateVModel model)
        {
            var entity = await _context.TimeOff.FindAsync(model.Id);
            if (entity != null)
            {
                entity = _mapper.Map(model, entity);
                _context.TimeOff.Update(entity); 
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "TimeOff"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public async Task ChangeStatus(int id)
        {
            var entity = await _context.TimeOff.FindAsync(id); 
            if (entity != null)
            {
                entity.IsActive = !entity.IsActive;
                _context.TimeOff.Update(entity); 
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "TimeOff"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public async Task Remove(int id)
        {
            var entity = await _context.TimeOff.FindAsync(id); 
            if (entity != null)
            {
                _context.TimeOff.Remove(entity); 
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, "TimeOff"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

       
    }
}
