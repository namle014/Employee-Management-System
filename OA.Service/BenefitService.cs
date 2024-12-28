using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels;
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
        private DbSet<BenefitType> _benefitType;
        private DbSet<BenefitUser> _benefitUser;
        private readonly UserManager<AspNetUser> _userManager;
        //private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Benefit";

        public BenefitService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor, UserManager<AspNetUser> userManager) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _benefit = dbContext.Set<Benefit>();
            _benefitType = dbContext.Set<BenefitType>();
            _benefitUser = dbContext.Set<BenefitUser>();
            //_userManager = userManager;
            _mapper = mapper;
            _userManager = userManager;
            _userManager = userManager;
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

        public async Task<ResponseResult> GetAll(FilterBenefitVModel model)
        {
            var result = new ResponseResult();
            var query = _benefit.AsQueryable();
            //var holidayList = await query.ToListAsync();
            string? keyword = model.Keyword?.ToLower();

            var records = await _benefit.Where(x =>
                x.IsActive == model.IsActive &&
                (!model.CreatedDate.HasValue ||
                    (x.CreatedDate.HasValue &&
                    x.CreatedDate.Value.Date == model.CreatedDate.Value.Date)) &&
                (string.IsNullOrEmpty(keyword) ||
                    x.Name.ToLower().Contains(keyword.ToLower()) ||
                    (x.Id != null && x.Id.ToLower().Contains(keyword.ToLower())) ||
                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword.ToLower()))
                )).ToListAsync();


            if (model.IsDescending == false)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderBy(r => r.CreatedDate).ToList()
                        : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderByDescending(r => r.CreatedDate).ToList()
                        : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }


            result.Data = new Pagination();
            var list = new List<BenefitGetAllVModel>();
            foreach (var entity in records)
            {
                var vmodel = _mapper.Map<BenefitGetAllVModel>(entity);

                var entity1 = await _benefit
                .Include(i => i.BenefitType)
                .FirstOrDefaultAsync(i => i.Id == entity.Id);
                if(entity1 != null)
                {
                    vmodel.NameOfBenefitType = entity1.BenefitType.Name;
                }

                list.Add(vmodel);
            }
            var pagedRecords = list.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            result.Data.Records = pagedRecords;
            result.Data.TotalRecords = list.Count;

            return result;
        }

        public async Task Create(BenefitCreateVModel model)
        {
            var benefit = _mapper.Map<BenefitCreateVModel, Benefit>(model);
            benefit.Id = await SetIdMax(model);
            benefit.CreatedDate = DateTime.UtcNow;
            benefit.CreatedBy = GlobalUserName;
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
            entity.UpdatedBy = GlobalUserName;

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



        //public async Task<ResponseResult> GetAll()
        //{
        //    var result = new ResponseResult();
        //    var data = _benefit.AsQueryable();
        //    result.Data = await data.ToListAsync();
        //    return result;
        //}
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

        public async Task ChangeStatusMany(BenefitChangeStatusManyVModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Lấy danh sách các thực thể cần cập nhật
                var entitiesToUpdate = await _benefit.Where(x => model.Ids.Contains(x.Id)).ToListAsync();

                // Kiểm tra xem có thiếu ID nào không
                var missingIds = model.Ids.Except(entitiesToUpdate.Select(x => x.Id)).ToList();
                if (missingIds.Any())
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, string.Join(", ", missingIds)));
                }

                // Cập nhật giá trị IsActive
                foreach (var entity in entitiesToUpdate)
                {
                    entity.IsActive = !entity.IsActive; // Đảo ngược trạng thái IsActive
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _dbContext.SaveChangesAsync();

                // Commit giao dịch
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback nếu xảy ra lỗi
                await transaction.RollbackAsync();
                throw new BadRequestException($"Transaction failed: {ex.Message}");
            }
        }

        public async Task<ResponseResult> GetAllBenefitType()
        {
            var result = new ResponseResult();

            // Lấy toàn bộ danh sách BenefitType từ DbContext
            var records = await _dbContext.BenefitType
                .OrderBy(x => x.Id) // Sắp xếp theo tên, có thể bỏ nếu không cần
                .ToListAsync();

            // Map sang ViewModel sử dụng AutoMapper
            var list = _mapper.Map<List<BenefitTypeGetAllVModel>>(records);

            // Gán vào response
            result.Data = new Pagination
            {
                Records = list,
                TotalRecords = list.Count
            };

            return result;
        }

        public async Task CreateBenefitType(BenefitTypeCreateVModel model)
        {
            var benefitType = _mapper.Map<BenefitTypeCreateVModel, BenefitType>(model);
            //benefitType.Name = model.Name;
            //benefitType.Description = model.Description;

            _dbContext.BenefitType.Add(benefitType);

            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateBenefitUser(CreateBenefitUser model)
        {
            var benefitUser = _mapper.Map<CreateBenefitUser, BenefitUser>(model);
            //benefitType.Name = model.Name;
            //benefitType.Description = model.Description;

            _dbContext.BenefitUser.Add(benefitUser);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateBenefitType(BenefitTypeUpdateVModel model)
        {
            var entity = await _benefitType.FindAsync(model.Id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            entity.Name = model.Name;
            entity.Description = model.Description;

            _mapper.Map(model, entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task RemoveBenefitType(string id)
        {
            var entity = await _benefitType.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _benefitType.Remove(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task<ResponseResult> GetAllBenefitUser(GetAllBenefitUser model)
        {
            var result = new ResponseResult();
            var query = _benefitUser.AsQueryable();
            string? keyword = model.Keyword?.ToLower();

            var records = await _benefitUser.ToListAsync();


            if (model.IsDescending == false)
            {
                //records = string.IsNullOrEmpty(model.SortBy).records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                //records = string.IsNullOrEmpty(model.SortBy)
                //        ? records.OrderByDescending(r => r.CreatedDate).ToList()
                //        : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }


            result.Data = new Pagination();
            var list = new List<GetAllBenefitUser>();
            foreach (var entity in records)
            {
                var vmodel = _mapper.Map<GetAllBenefitUser>(entity);

                var userId = entity.UserId;
                var benefitId = entity.BenefitId;

                var usertable = await _dbContext.AspNetUsers
                    .Where(x=>x.Id == userId).ToListAsync();
                vmodel.FullName = usertable[0].FullName;
                vmodel.Gender = usertable[0].Gender;
                var roles = await _userManager.GetRolesAsync(usertable[0]);
                vmodel.Roles = roles.ToList();

                int? departmentId = usertable[0].DepartmentId;
                var departmentName = await _dbContext.Department.Where(x=> x.Id == departmentId).Select(x => x.Name).FirstOrDefaultAsync();
                vmodel.DepartmentName = departmentName;

                var benefitName = await _dbContext.Benefit.Where(x => x.Id == benefitId).Select(x => x.Name).FirstOrDefaultAsync();
                if(benefitName != null)
                {
                    vmodel.BenefitName = benefitName;
                }

                if (
                    (string.IsNullOrEmpty(keyword) ||
                     (vmodel.FullName?.ToLower()?.Contains(keyword) == true) ||
                     (vmodel.BenefitName?.ToLower()?.Contains(keyword) == true) ||
                     (vmodel.UserId?.ToLower()?.Contains(keyword) == true) ||
                     (vmodel.Roles != null && vmodel.Roles.Any(role => role.ToLower().Contains(keyword))) 
                     //(model.DepartmentIds != null && departmentId != null && model.DepartmentIds.Any(id => id == departmentId)) ||
                    ) &&
                    (model.Roles.Count == 0||(model.Roles.Count != 0 && vmodel.Roles.Count !=0 && model.Roles.Any(role => vmodel.Roles.Contains(role)))) &&
                    (model.DepartmentIds.Count == 0 || (model.DepartmentIds.Count != 0 && departmentId != null && model.DepartmentIds.Any(id => id == departmentId))) &&
                    (model.Gender == null || vmodel.Gender == model.Gender) &&
                    (vmodel.BenefitContribution >= model.FromBenefitContribution &&
                     vmodel.BenefitContribution < model.ToBenefitContribution)
                   )

                list.Add(vmodel);
            }
            var pagedRecords = list.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            result.Data.Records = pagedRecords;
            result.Data.TotalRecords = list.Count;

            return result;
        }
    }
}
