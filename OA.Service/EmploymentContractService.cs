using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.VModels;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Infrastructure.EF.Context;
using OA.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OA.Core.Services;

namespace OA.Service
{
    public class EmploymentContractService : IEmploymentContractService
    {
        private readonly ApplicationDbContext _context; 
        private readonly IMapper _mapper;

        public EmploymentContractService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async Task<ExportStream> ExportFile(FilterEmploymentContractVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<EmploymentContractExportVModel>>(result.Data.Records);
            var exportData = ImportExportHelper<EmploymentContractExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }


        public async Task<ResponseResult> GetById(int id)
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
            var maxId = await _context.EmploymentContract.MaxAsync(x => (int?)x.Id) ?? 0; 
            entityCreated.Id = maxId + 1;
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

        public async Task ChangeStatus(int id)
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

        public async Task Remove(int id)
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
