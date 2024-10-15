using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        private readonly IMapper _mapper;
        string _nameService = "Timekeeping";

        public TimekeepingService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _timekeepings = dbContext.Set<Timekeeping>();
            _mapper = mapper;
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

            result.Data = timekeepingList;

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

            result.Data = _mapper.Map<Timekeeping, TimekeepingGetByIdVModel>(entity);

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
