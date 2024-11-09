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
    public class TimekeepingService : GlobalVariables, ITimekeepingService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Timekeeping> _timekeepings;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Timekeeping";
        private DbSet<Department> _departments;

        public TimekeepingService(ApplicationDbContext dbContext, UserManager<AspNetUser> userManager, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _timekeepings = dbContext.Set<Timekeeping>();
            _departments = dbContext.Set<Department>();
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ResponseResult> GetAllDepartments()
        {
            var result = new ResponseResult();
            var data = _departments.AsQueryable();
            result.Data = await data.ToListAsync();
            return result;
        }

        public async Task<ResponseResult> Search(FilterTimekeepingVModel model)
        {
            var result = new ResponseResult();

            var query = _timekeepings.AsQueryable();

            if (!string.IsNullOrEmpty(model.UserId))
            {
                query = query.Where(t => t.UserId == model.UserId);
            }

            if (model.StartDate.HasValue)
            {
                query = query.Where(t => t.Date >= model.StartDate.Value);
            }

            if (model.EndDate.HasValue)
            {
                query = query.Where(t => t.Date <= model.EndDate.Value);
            }

            if (model.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == model.IsActive.Value);
            }

            var timekeepingList = await query.ToListAsync();

            var timekeepingGrouped = timekeepingList.GroupBy(t => t.UserId);

            var timekeepingListMapped = new List<TimekeepingGetAllVModel>();

            foreach (var group in timekeepingGrouped)
            {
                var user = await _userManager.FindByIdAsync(group.Key);
                if (user == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, $"UserId = {group.Key}"));
                }

                foreach (var timekeeping in group)
                {
                    var entityMapped = _mapper.Map<Timekeeping, TimekeepingGetAllVModel>(timekeeping);
                    entityMapped.FullName = user.FullName;
                    timekeepingListMapped.Add(entityMapped);
                }
            }

            result.Data = timekeepingListMapped;

            return result;
        }

        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();

            var entity = await _timekeepings.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            var user = await _userManager.FindByIdAsync(entity.UserId);
            if (user == null)
            {
                throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, "User"));
            }

            var entityMapped = _mapper.Map<Timekeeping, TimekeepingGetByIdVModel>(entity);
            entityMapped.FullName = user.FullName;

            result.Data = entityMapped;

            return result;
        }

        public async Task Create(TimekeepingCreateVModel model)
        {
            var entity = _mapper.Map<TimekeepingCreateVModel, Timekeeping>(model);

            entity.IsActive = CommonConstants.Status.Active;

            _timekeepings.Add(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
            }
        }

        public async Task Update(TimekeepingUpdateVModel model)
        {
            var entity = await _timekeepings.FindAsync(model.Id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _mapper.Map(model, entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task ChangeStatus(int id)
        {
            var entity = await _timekeepings.FindAsync(id);
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

        public async Task Remove(int id)
        {

            var entity = await _timekeepings.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _timekeepings.Remove(entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }
    }
}
