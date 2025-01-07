using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;

namespace OA.Service
{
    public class JobHistoryService : GlobalVariables, IJobHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AspNetUser> _userManager;


        public JobHistoryService(UserManager<AspNetUser> userManager, IHttpContextAccessor contextAccessor, ApplicationDbContext context, IMapper mapper) : base(contextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // Search for error reports with optional filtering
        public async Task<ResponseResult> Search(FilterJobHistoryVModel model)
        {
            var result = new ResponseResult();
            var recordsQuery = _context.JobHistory.AsQueryable();

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

        // Export error reports to a file
        public async Task<ExportStream> ExportFile(FilterJobHistoryVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<JobHistoryExportVModel>>(result.Data.Records);
            var exportData = ImportExportHelper<JobHistoryExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }

        // Get a specific error report by Id
        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();
            var entity = await _context.JobHistory.FindAsync(id);
            if (entity != null)
            {
                result.Data = _mapper.Map<JobHistory>(entity);
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            return result;
        }



        public async Task Create(JobHistoryVModel model)
        {
          
            List<string> usersToNotify;

            if (model.TypeToNotify == 1)
            {
                var users = await _userManager.Users.ToListAsync();
                usersToNotify = users.Select(user => user.Id).ToList();
            }
            else
            {
                if (model.ListUser == null || !model.ListUser.Any())
                {
                    throw new BadRequestException("ListUser cannot be null or empty.");
                }
                usersToNotify = model.ListUser;
            }

        
            foreach (var userId in usersToNotify)
            {
                var jobHistory = new JobHistoryCreateVModel
                {
                    EmployeeId = userId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Note = model.Note,
                    JobDescription = model.JobDescription,
                    SupervisorId = GlobalUserId,
                    WorkLocation = model.WorkLocation,
                    Allowance = model.Allowance
                };
                var entity = _mapper.Map<JobHistoryCreateVModel, JobHistory>(jobHistory);
                await _context.JobHistory.AddAsync(entity);
            }

            var saveResult = await _context.SaveChangesAsync();
            if (saveResult <= 0)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "JobHistory"));
            }
        }





        // Update an existing error report
        public async Task Update(JobHistoryUpdateVModel model)
        {
            var entity = await _context.JobHistory.FindAsync(model.Id);
            if (entity != null)
            {
                entity = _mapper.Map(model, entity);
                _context.JobHistory.Update(entity);
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, "JobHistory"));
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
            var entity = await _context.JobHistory.FindAsync(id);
            if (entity != null)
            {
                _context.JobHistory.Remove(entity);
                var saveResult = await _context.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, "JobHistory"));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }
    }
}
