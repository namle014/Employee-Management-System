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
    public class BenefitService : GlobalVariables, IBenefitService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Benefit> _benefit;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Benefit";

        public BenefitService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _benefit = dbContext.Set<Benefit>();
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ResponseResult> GetById(string id)
        {
            var result = new ResponseResult();

            var entity = await _benefit
                .Include(i => i.BenefitType)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var entityMapped = _mapper.Map<Benefit, BenefitGetByIdVModel>(entity);

            if (entity.BenefitType != null)
            {
                entityMapped.BenefitTypeId = entity.BenefitType.Id;
                entityMapped.NameOfBenefitType = entity.BenefitType.Name;
            }

            result.Data = entityMapped;

            return result;
        }

        public async Task<ResponseResult> Search(FilterBenefitVModel model)
        {
            var result = new ResponseResult();
            var query = _benefit.AsQueryable();

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

            if (!CheckIsNullOrEmpty(model.Keyword))
            {
                string keyword = model.Keyword.ToLower();
                query = query.Where(t => (t.Name.ToLower().Contains(keyword) == true) ||
                                         (t.CreatedBy != null && t.CreatedBy.ToLower().Contains(keyword)) ||
                                         (t.UpdatedBy != null && t.UpdatedBy.ToLower().Contains(keyword)));
            }


            var benefitList = await query.ToListAsync();
            var benefitGrouped = benefitList.GroupBy(t => t.Id);

            var benefitListMapped = new List<BenefitGetAllVModel>();

            foreach (var group in benefitGrouped)
            {
                foreach (var insurance in group)
                {
                    var entity = await _benefit
                .Include(i => i.BenefitType)
                .FirstOrDefaultAsync(i => i.Id == insurance.Id);
                    if (entity == null)
                    {
                        throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                    }

                    var entityMapped = _mapper.Map<Benefit, BenefitGetAllVModel>(entity);

                    entityMapped.NameOfBenefitType = entity.BenefitType.Name;
                    benefitListMapped.Add(entityMapped);
                }
            }

            result.Data = benefitListMapped;

            return result;
        }

        public async Task Create(BenefitCreateVModel model)
        {
            var benefit = _mapper.Map<BenefitCreateVModel, Benefit>(model);
            benefit.Id = await SetIdMax(model);
            benefit.CreatedDate = DateTime.UtcNow;
            benefit.IsActive = CommonConstants.Status.Active;

            _dbContext.Benefit.Add(benefit);

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(BenefitUpdateVModel model)
        {
            var entity = await _benefit.FindAsync(model.Id);
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
            var entity = await _benefit.FindAsync(id);
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
            var entity = await _benefit.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _benefit.Remove(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task<ResponseResult> GetAll()
        {
            var result = new ResponseResult();
            var data = _benefit.AsQueryable();
            result.Data = await data.ToListAsync();
            return result;
        }
        public virtual bool CheckIsNullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value)) return true;
            return false;
        }
        public async Task<string> SetIdMax(BenefitCreateVModel model)
        {
            var entity = _mapper.Map<BenefitCreateVModel, Benefit>(model);
            var idList = await _benefit.Select(x => x.Id).ToListAsync();

            var highestId = idList.Select(id => new
            {
                originalId = id,
                numPart = int.TryParse(id.Substring(2), out int number) ? number : -1
            })
            .OrderByDescending(x => x.numPart).Select(x => x.originalId).FirstOrDefault();

            if (highestId != null)
            {
                if (highestId.Length > 2 && highestId.StartsWith("BE"))
                {
                    var newIdNumber = int.Parse(highestId.Substring(2)) + 1;
                    entity.Id = "BE" + newIdNumber.ToString("D3");
                    return entity.Id;
                }
                else
                {
                    throw new InvalidOperationException("Invalid ID format in the database.");
                }
            }
            else
            {
                entity.Id = "BE001";
                return entity.Id;

            }
        }
    }
}
