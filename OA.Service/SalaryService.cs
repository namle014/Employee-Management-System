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
                
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }

        public async Task Update(SalaryUpdateVModel model)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            throw new NotImplementedException();
        }
    }
}
