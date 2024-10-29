using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;

namespace OA.Service
{
    public class BenefitService : GlobalVariables, IBenefitService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Benefit> _insurance;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        // string _nameService = "Insurance";

        public BenefitService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _insurance = dbContext.Set<Benefit>();
            _userManager = userManager;
            _mapper = mapper;
        }

        public Task<ResponseResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseResult> Search(FilterBenefitVModel model)
        {
            throw new NotImplementedException();
        }

        public async Task Create(BenefitCreateVModel model)
        {
            var benefit = _mapper.Map<Benefit>(model);

            benefit.CreatedDate = DateTime.UtcNow; // Gán giá trị CreatedDate
            _dbContext.Benefit.Add(benefit);    // Thêm insurance vào DbSet

            await _dbContext.SaveChangesAsync();
            // return Task.CompletedTask;// Lưu thay đổi vào database
        }

        public Task Update(BenefitUpdateVModel model)
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
