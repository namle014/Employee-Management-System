﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;

namespace OA.Service
{
    public class InsuranceService : GlobalVariables, IInsuranceService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Insurance> _insurance;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Timekeeping";

        public InsuranceService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _insurance = dbContext.Set<Insurance>();
            _userManager = userManager;
            _mapper = mapper;
        }

        public Task<ResponseResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseResult> Search(FilterInsuranceVModel model)
        {
            throw new NotImplementedException();
        }

        public Task Create(InsuranceCreateVModel model)
        {
            throw new NotImplementedException();
        }

        public Task Update(InsuranceUpdateVModel model)
        {
            throw new NotImplementedException();
        }

        public Task ChangeStatus(int id)
        {
            throw new NotImplementedException();
        }

        public Task Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
