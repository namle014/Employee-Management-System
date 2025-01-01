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
    public class HolidayService : GlobalVariables, IHolidayService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Events> _event;
        private readonly IMapper _mapper;
        private readonly string _nameService = "Event";
        public HolidayService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _mapper = mapper;
            _event = dbContext.Set<Events>();
        }

        public async Task Create(EventCreateVModel model)
        {
            var entity = _mapper.Map<EventCreateVModel, Events>(model);
            _event.Add(entity);
            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));


            }
        }
        
        public async Task DeleteMany(HolidayDeleteManyVModel model)
        {
            if (model?.Ids == null || !model.Ids.Any())
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Lấy danh sách các thực thể cần xóa
                var entitiesToDelete = await _event.Where(x => model.Ids.Contains(x.Id)).ToListAsync();

                // Kiểm tra xem có thiếu ID nào không
                var missingIds = model.Ids.Except(entitiesToDelete.Select(x => x.Id)).ToList();
                if (missingIds.Any())
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, string.Join(", ", missingIds)));
                }

                // Xóa các thực thể
                _event.RemoveRange(entitiesToDelete);

                // Lưu thay đổi
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

        public async Task<ResponseResult> GetAll(EventFilterVModel model)
        {
            var result = new ResponseResult();
            var query = _event.AsQueryable();

            string? keyword = model.Keyword?.ToLower();

            var records = await _event.Where(x => x.IsHoliday &&
                (string.IsNullOrEmpty(keyword) ||
                    x.Title.ToLower().Contains(keyword) ||
                    (x.Description != null && x.Description.ToLower().Contains(keyword))
                )).ToListAsync();

                
            if (model.IsDescending == false)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderBy(r => r.Title).ToList()
                        : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderByDescending(r => r.Title).ToList()
                        : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            result.Data = new Pagination();

            var pagedRecords = records.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            result.Data.Records = pagedRecords;
            result.Data.TotalRecords = records.Count;

            return result;
        }

        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();
            try
            {
                var entity = await _event.FirstOrDefaultAsync(s => s.Id == id);
                if (entity == null)
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                }

                result.Data = entity;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }

        public async Task Remove(int id)
        {
            var entity = await _event.FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _event.Remove(entity);
            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task Update(EventUpdateVModel model)
        {
            var entity = await _event.FindAsync(model.Id);
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

    }
}
