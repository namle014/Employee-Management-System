using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using OA.Core.Constants;
using OA.Core.Models;
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

        public ErrorReportService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Search for error reports with optional filtering
        public async Task<ResponseResult> Search(FilterErrorReportVModel model)
        {
            var result = new ResponseResult();
            var recordsQuery = _context.ErrorReport.AsQueryable();

            if (!string.IsNullOrEmpty(model.Status))
            {
                recordsQuery = recordsQuery.Where(x => x.Status == model.Status);
            }

            if (!string.IsNullOrEmpty(model.Keyword))
            {
                var keyword = model.Keyword.ToLower();
                recordsQuery = recordsQuery.Where(x =>
                    x.ReportedBy.ToLower().Contains(keyword) ||
                    x.Description.ToLower().Contains(keyword) ||
                    x.Status.ToLower().Contains(keyword)
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
