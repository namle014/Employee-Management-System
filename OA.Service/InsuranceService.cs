using AutoMapper;
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
        private string _nameService = "Insurance";
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;

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
                entityMapped.NameOfInsuranceType = entity.InsuranceType.Name; 
            }

            result.Data = entityMapped;

            return result;
        }


        public async Task<ResponseResult> Search(FilterInsuranceVModel model)
        {
            var result = new ResponseResult();
            var query = _insurance.AsQueryable();

            if (!string.IsNullOrEmpty(model.Id))
            {
                query = query.Where(t => t.Id.StartsWith(model.Id));
            }

            if (model.StartDate.HasValue)
            {
                query = query.Where(t =>
                    (t.UpdatedDate.HasValue ? t.UpdatedDate.Value : t.CreatedDate) >= model.StartDate.Value);
            }

            if (model.EndDate.HasValue)
            {
                query = query.Where(t =>
                    (t.UpdatedDate.HasValue ? t.UpdatedDate.Value : t.CreatedDate) <= model.EndDate.Value);
            }

            if (model.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == model.IsActive.Value);
            }

            if(!CheckIsNullOrEmpty(model.Keyword))
            {
                string keyword = model.Keyword.ToLower();
                query = query.Where(t => (t.Name.ToLower().Contains(keyword) == true) || 
                                         (t.CreatedBy != null && t.CreatedBy.ToLower().Contains(keyword)) ||
                                         (t.UpdatedBy != null && t.UpdatedBy.ToLower().Contains(keyword)));
            }


            var insuranceList = await query.ToListAsync();
            var insuranceGrouped = insuranceList.GroupBy(t => t.Id);

            var insuranceListMapped = new List<InsuranceGetAllVModel>();

            foreach (var group in insuranceGrouped)
            {
                foreach (var insurance in group)
                {
                    var entity = await _insurance
                .Include(i => i.InsuranceType)
                .FirstOrDefaultAsync(i => i.Id == insurance.Id);
                    if (entity == null)
                    {
                        throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                    }

                    var entityMapped = _mapper.Map<Insurance, InsuranceGetAllVModel>(entity);
                    
                    entityMapped.NameOfInsuranceType = entity.InsuranceType.Name; 
                    insuranceListMapped.Add(entityMapped);
                }
            }

            result.Data = insuranceListMapped;

            return result;
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


        public async Task ChangeStatus(string id)
        {
            var entity = await _insurance.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            entity.IsActive = !entity.IsActive;

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task Remove(string id)
        {
            var entity = await _insurance.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _insurance.Remove(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task<ResponseResult> GetAll()
        {
            var result = new ResponseResult();
            var data = _insurance.AsQueryable();
            result.Data = await data.ToListAsync();
            return result;
        }

        public async Task<string> SetIdMax(InsuranceCreateVModel model)
        {
            var entity = _mapper.Map<InsuranceCreateVModel, Insurance>(model);
            var idList = await _insurance.Select(x => x.Id).ToListAsync();

            var highestId = idList.Select(id => new
            {
                originalId = id,
                numPart = int.TryParse(id.Substring(2), out int number) ? number : -1 
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

        public virtual bool CheckIsNullOrEmpty(string value)
        {
            if(string.IsNullOrEmpty(value)) return true;
            return false;
        }
        
    }
}
