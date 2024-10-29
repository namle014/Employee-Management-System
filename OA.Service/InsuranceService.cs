using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
        private string _nameService = "Insurance";
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
       // string _nameService = "Insurance";

        public InsuranceService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _insurance = dbContext.Set<Insurance>();
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ResponseResult> GetById(string id)
        {
            var result = new ResponseResult();
       //     var insurances = _dbContext.Insurance
       //.Include(i => i.InsuranceType) 
       //.ToList();

            var entity = await _insurance
                .Include(i => i.InsuranceType) 
                .FirstOrDefaultAsync(i => i.Id == id); 

            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var entityMapped = _mapper.Map<Insurance, InsuranceGetByIdVModel>(entity);

            if (entity.InsuranceType != null)
            {
                entityMapped.InsuranceTypeId = entity.InsuranceType.Id; 
                entityMapped.Name = entity.InsuranceType.Name; 
            }

            result.Data = entityMapped;

            return result;
        }


        public Task<ResponseResult> Search(FilterInsuranceVModel model)
        {
            throw new NotImplementedException();
        }

        public async Task Create(InsuranceCreateVModel model)
        {
            var insurance = _mapper.Map<InsuranceCreateVModel, Insurance>(model);
            insurance.Id = await SetIdMax(model);
            insurance.CreatedDate = DateTime.UtcNow;
            insurance.IsActive = CommonConstants.Status.Active;

            _dbContext.Insurance.Add(insurance);    

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(InsuranceUpdateVModel model)
        {
            var entity = await _insurance.FindAsync(model.Id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            entity.UpdatedDate = DateTime.Now;

            _mapper.Map(model, entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }


        public Task ChangeStatus(int id)
        {
            throw new NotImplementedException();
        }

        public Task Remove(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SetIdMax(InsuranceCreateVModel model)
        {
            var entity = _mapper.Map<InsuranceCreateVModel, Insurance>(model);
            var idList = await _insurance.Select(x => x.Id).ToListAsync();

            var highestId = idList.Select(id => new
            {
                originalId = id,
                numPart = int.TryParse(id.Substring(2), out int number) ? number : -1 //nv001
            })
            .OrderByDescending(x => x.numPart).Select(x => x.originalId).FirstOrDefault();

            if (highestId != null)
            {
                if (highestId.Length > 2 && highestId.StartsWith("IN"))
                {
                    var newIdNumber = int.Parse(highestId.Substring(2)) + 1;
                    entity.Id = "IN" + newIdNumber.ToString("D3");
                    return entity.Id;
                }
                else
                {
                    throw new InvalidOperationException("Invalid ID format in the database.");
                }
            }
            else
            {
                entity.Id = "IN001";
                return entity.Id;

            }
        }

        public async Task<ResponseResult> GetAll()
        {
            //throw new NotImplementedException();
            var result = new ResponseResult();
            var data = _insurance.AsQueryable();
            result.Data = await data.ToListAsync();
            return result;
        }
    }
}
