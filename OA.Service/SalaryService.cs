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


namespace OA.Service
{
    public class SalaryService : GlobalVariables, ISalaryService
    {
        public SalaryService(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        public Task Create(SalaryCreateVModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseResult> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task Update(SalaryUpdateVModel model)
        {
            throw new NotImplementedException();
        }
    }
}
