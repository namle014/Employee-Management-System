using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;

namespace OA.Service
{
    public class ErrorReportService : IErrorReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly IBaseRepository<SysFile> _sysFileRepo;
        private readonly IBaseRepository<Department> _departmentService;

        public ErrorReportService(IBaseRepository<Department> departmentService, IBaseRepository<SysFile> sysFileRepo, RoleManager<AspNetRole> roleManager, UserManager<AspNetUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _sysFileRepo = sysFileRepo;
            _departmentService = departmentService;
        }


        public async Task<ResponseResult> Search(FilterErrorReportVModel model)
        {
            var result = new ResponseResult();
            string? keyword = model.Keyword?.ToLower();

            string? isType = model.IsType;

            var recordsQuery = await _context.ErrorReport.ToListAsync();

            if (!string.IsNullOrEmpty(isType))
            {
                recordsQuery= recordsQuery.Where(x =>x.Type == isType).ToList();
            }
            

            var recordsWithDetails = new List<dynamic>();

            foreach (var ErrorReport in recordsQuery)
            {
                if (ErrorReport.ReportedBy != null)
                {
                    var user = await _userManager.FindByIdAsync(ErrorReport.ReportedBy);
                    var manager = await _userManager.FindByIdAsync(ErrorReport.ResolvedBy);
                    if (user != null)
                    {


                        var userAvatarPath = "https://localhost:44381/avatars/aa1678f0-75b0-48d2-ae98-50871178e9bd.jfif";
                        if (user.AvatarFileId.HasValue)
                        {
                            var sysFile = await _sysFileRepo.GetById((int)user.AvatarFileId);
                            if (sysFile != null)
                            {
                                userAvatarPath = "https://localhost:44381/" + sysFile.Path;
                            }
                        }

                        var managerAvatarPath = "https://localhost:44381/avatars/aa1678f0-75b0-48d2-ae98-50871178e9bd.jfif";

                        if (manager != null)
                        {
                            if (manager.AvatarFileId.HasValue)
                            {
                                var sysFile = await _sysFileRepo.GetById((int)manager.AvatarFileId);
                                if (sysFile != null)
                                {
                                    managerAvatarPath = "https://localhost:44381/" + sysFile.Path;
                                }
                            }
                        }


                        dynamic ErrorReportModel = new
                        {
                            Id = ErrorReport.Id,
                            ReportedBy = ErrorReport.ReportedBy,
                            ReportedDate = ErrorReport.ReportedDate,
                            Type = ErrorReport.Type,
                            TypeId = ErrorReport.TypeId,
                            Description = ErrorReport.Description,
                            Status = ErrorReport.Status,
                            ResolvedBy = ErrorReport.ResolvedBy,
                            ResolvedDate = ErrorReport.ResolvedDate,
                            ResolutionDetails = ErrorReport.ResolutionDetails,
                            ReportedFullName = user.FullName,
                            ReportedId = user.EmployeeId,
                            ReportedAvatarPath = userAvatarPath,
                            ResolvedFullName = manager?.FullName,
                            ResolvedId = manager?.EmployeeId,
                            ResolvedAvatarPath = managerAvatarPath,
                        };
                        recordsWithDetails.Add(ErrorReportModel);
                    }
                }
            }


            if (!string.IsNullOrEmpty(keyword))
            {
                string lowerKeyword = keyword.ToLower();
                recordsWithDetails = recordsWithDetails.Where(x =>
                    (x.ReportedBy != null && x.ReportedBy.ToLower().Contains(lowerKeyword)) ||
                    (x.TypeId != null && x.TypeId.ToLower().Contains(lowerKeyword)) ||
                    (x.Description != null && x.Description.ToLower().Contains(lowerKeyword)) ||
                    (x.ResolvedBy != null && x.ResolvedBy.ToLower().Contains(lowerKeyword)) ||
                    (x.ResolutionDetails != null && x.ResolutionDetails.ToLower().Contains(lowerKeyword)) ||
                    (x.Type != null && x.Type.ToLower().Contains(lowerKeyword))||
                    (x.ReportedFullName != null && x.ReportedFullName.ToLower().Contains(lowerKeyword))||
                    (x.ReportedId != null && x.ReportedId.ToLower().Contains(lowerKeyword))||
                    (x.ResolvedFullName != null && x.ResolvedFullName.ToLower().Contains(lowerKeyword))||
                    (x.ResolvedId != null && x.ResolvedId.ToLower().Contains(lowerKeyword))
                ).ToList();
            }

            var records = recordsWithDetails.ToList();

            if (model.IsDescending == false)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderBy(r => r.ReportedDate).ToList()
                        : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderByDescending(r => r.ReportedDate).ToList()
                        : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            result.Data = new Pagination()
            {
                Records = records.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList(),
                TotalRecords = records.Count()
            };

            return result;
        }


        public async Task<ResponseResult> SearchByUserId(FilterErrorReportVModel model, string UserId)
        {
            var result = new ResponseResult();
            string? keyword = model.Keyword?.ToLower();
            string? isType = model.IsType;

            var recordsQuery = _context.ErrorReport.AsQueryable().Where(x => x.ReportedBy == UserId);

            if (!string.IsNullOrEmpty(isType))
            {
                recordsQuery = recordsQuery.Where(x => x.Type == isType);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                string lowerKeyword = keyword.ToLower();

                recordsQuery = recordsQuery.Where(x =>
                    (x.ReportedBy != null && x.ReportedBy.ToLower().Contains(lowerKeyword)) ||
                    (x.TypeId != null && x.TypeId.ToLower().Contains(lowerKeyword)) ||
                    (x.Description != null && x.Description.ToLower().Contains(lowerKeyword)) ||
                    (x.ResolvedBy != null && x.ResolvedBy.ToLower().Contains(lowerKeyword)) ||
                    (x.ResolutionDetails != null && x.ResolutionDetails.ToLower().Contains(lowerKeyword)) ||
                    (x.Type != null && x.Type.ToLower().Contains(lowerKeyword))
                );
            }


            var records = recordsQuery.ToList();

            if (model.IsDescending == false)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderBy(r => r.ReportedDate).ToList()
                        : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderByDescending(r => r.ReportedDate).ToList()
                        : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            result.Data = new Pagination()
            {
                Records = records.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList(),
                TotalRecords = records.Count()
            };

            return result;
        }



        public async Task<ResponseResult> CountErrorReportsByStatusAndMonth(int year)
        {
            var result = new ResponseResult();
          
            var errorReports = await _context.ErrorReport
                .Where(x => x.ReportedDate.HasValue && x.ReportedDate.Value.Year == year)
                .GroupBy(x => x.ReportedDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Unprocessed = g.Count(x => x.Status == "1" || x.Status == "0"),
                    Processed = g.Count(x => x.Status == "2" || x.Status == "3")
                })
                .ToListAsync();

            var resultData = Enumerable.Range(1, 12).Select(month => new
            {
                Month = month,
                Unprocessed = errorReports.FirstOrDefault(x => x.Month == month)?.Unprocessed ?? 0,
                Processed = errorReports.FirstOrDefault(x => x.Month == month)?.Processed ?? 0
            }).ToArray();

            result.Data = resultData;

            return result;
        }




        public async Task<ResponseResult> CountErrorReportsInMonth(int year, int month)
        {
            var result = new ResponseResult();

            var currentMonthCount = await _context.ErrorReport
                .Where(x => x.ReportedDate.HasValue && x.ReportedDate.Value.Year == year && x.ReportedDate.Value.Month == month)
                .CountAsync();

            var previousMonth = month == 1 ? 12 : month - 1;
            var previousYear = month == 1 ? year - 1 : year;
            var previousMonthCount = await _context.ErrorReport
                .Where(x => x.ReportedDate.HasValue && x.ReportedDate.Value.Year == previousYear && x.ReportedDate.Value.Month == previousMonth)
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

        public async Task<ResponseResult> CountErrorReportsByTypeAndYear(int year)
        {
            var result = new ResponseResult();         
            var errorReports = await _context.ErrorReport
                .Where(x => x.ReportedDate.HasValue && x.ReportedDate.Value.Year == year)
                .GroupBy(x => x.Type) 
                .Select(g => new
                {
                    Type = g.Key, 
                    Count = g.Count()  
                })
                .ToListAsync();

            result.Data = errorReports;

            return result;
        }


        // Export error reports to a file
        public async Task<ExportStream> ExportFile(FilterErrorReportVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<ErrorReportExportVModel>>(result.Data.Records);
            var exportData = ImportExportHelper<ErrorReportExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }

        // Get a specific error report by Id
        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();
            var entity = await _context.ErrorReport.FindAsync(id);
            if (entity != null)
            {
                result.Data = _mapper.Map<ErrorReport>(entity);
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            return result;
        }


        // Create a new error report
        public async Task Create(ErrorReportCreateVModel model)
        {
            
            
                var entityCreated = _mapper.Map<ErrorReportCreateVModel, ErrorReport>(model);

               
                await _context.ErrorReport.AddAsync(entityCreated);

                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "ErrorReport"));
                }
            
            
        }



        // Update an existing error report
        public async Task Update(ErrorReportUpdateVModel model)
        {
            var entity = await _context.ErrorReport.FindAsync(model.Id);
            if (entity != null)
            {
                entity = _mapper.Map(model, entity);
                _context.ErrorReport.Update(entity);
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "ErrorReport"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        // Remove an error report by Id
        public async Task Remove(int id)
        {
            var entity = await _context.ErrorReport.FindAsync(id);
            if (entity != null)
            {
                _context.ErrorReport.Remove(entity);
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, "ErrorReport"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }
    }
}
