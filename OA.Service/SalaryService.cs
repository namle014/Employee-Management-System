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


namespace OA.Service
{

    public class SalaryService : GlobalVariables, ISalaryService
    {
        private static BaseConnection _dbConnectSQL = BaseConnection.Instance();
        public SalaryService(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {

        }

        public Task Create(SalaryCreateVModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseResult> GetAll()
        {
            throw new NotImplementedException();
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
