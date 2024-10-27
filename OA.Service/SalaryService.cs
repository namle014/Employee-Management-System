using AutoMapper;
using OA.Core.Repositories;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OA.Domain.Services;
using OA.Core.Services;
using OA.Repository;
using Microsoft.AspNetCore.Http;
using OA.Core.Models;
using OA.Service.Helpers;
using OA.Core.Constants;
using OA.Infrastructure.SQL;
using Microsoft.EntityFrameworkCore;
using OA.Infrastructure.EF.Context;


namespace OA.Service
{

    public class SalaryService : GlobalVariables, ISalaryService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Salary> _salary;
        private readonly IMapper _mapper;
        private string _nameService = "Salary";

        public SalaryService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _salary = dbContext.Set<Salary>();
            _mapper = mapper;
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
                if (highestId.Length > 2 && highestId.StartsWith("nv"))
                {
                    var newIdNumber = int.Parse(highestId.Substring(2)) + 1; 
                    entity.Id = "nv" + newIdNumber.ToString("D3"); 
                }
                else
                {
                    throw new InvalidOperationException("Invalid ID format in the database.");
                }
            }
            else
            {
                entity.Id = "nv001";
            }
            entity.CreatedDate = DateTime.Now;
            entity.CreatedBy = GlobalUserName;
            entity.IsActive = true;
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
            var salaryList = await query.ToListAsync();
            result.Data = salaryList;
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
    }
}
