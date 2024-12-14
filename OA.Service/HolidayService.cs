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
        private readonly DbSet<Holiday> _holiday;
        private readonly IMapper _mapper;
        private readonly string _nameService = "holiday";
        public HolidayService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _mapper = mapper;
            _holiday = dbContext.Set<Holiday>();
        }

        public async Task Create(HolidayCreateVModel model)
        {
            var entity = _mapper.Map<HolidayCreateVModel, Holiday>(model);
            entity.StartDate = new DateTime(
                model.StartDate.GetValueOrDefault(DateTime.Now).Year,
                model.StartDate.GetValueOrDefault(DateTime.Now).Month,
                model.StartDate.GetValueOrDefault(DateTime.Now).Day,
                model.StartDate.GetValueOrDefault(DateTime.Now).Hour,
                model.StartDate.GetValueOrDefault(DateTime.Now).Minute,
                0
            );
            entity.EndDate = new DateTime(
                model.EndDate.GetValueOrDefault(DateTime.Now).Year,
                model.EndDate.GetValueOrDefault(DateTime.Now).Month,
                model.EndDate.GetValueOrDefault(DateTime.Now).Day,
                model.EndDate.GetValueOrDefault(DateTime.Now).Hour,
                model.EndDate.GetValueOrDefault(DateTime.Now).Minute,
                0
            );
            entity.CreatedDate = DateTime.Now;
            entity.CreatedBy = GlobalUserName;
            entity.IsActive = true;
            _holiday.Add(entity);
            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
            }
        }

        public async Task<ResponseResult> GetAll(HolidayFilterVModel model)
        {
            var result = new ResponseResult();
            var query = _holiday.AsQueryable();
            //var holidayList = await query.ToListAsync();
            string? keyword = model.Keyword?.ToLower();

            var records = await _holiday.Where(x =>
                x.IsActive == model.IsActive &&
                (!model.CreatedDate.HasValue ||
                    (x.CreatedDate.HasValue &&
                    x.CreatedDate.Value.Date == model.CreatedDate.Value.Date)) &&
                (string.IsNullOrEmpty(keyword) ||
                    x.Name.ToLower().Contains(keyword.ToLower()) ||
                    (x.Note != null && x.Note.ToLower().Contains(keyword.ToLower())) ||
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
            var list = new List<HolidayGetAllVModel>();
            foreach (var entity in records)
            {
                var vmodel = _mapper.Map<HolidayGetAllVModel>(entity);
                list.Add(vmodel);
            }
            var pagedRecords = list.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            result.Data.Records = pagedRecords;
            result.Data.TotalRecords = list.Count;

            return result;
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
                var entitiesToDelete = await _holiday.Where(x => model.Ids.Contains(x.Id)).ToListAsync();

                // Kiểm tra xem có thiếu ID nào không
                var missingIds = model.Ids.Except(entitiesToDelete.Select(x => x.Id)).ToList();
                if (missingIds.Any())
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, string.Join(", ", missingIds)));
                }

                // Xóa các thực thể
                _holiday.RemoveRange(entitiesToDelete);

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


        public async Task<ResponseResult> GetById(int id)
        {
            var result = new ResponseResult();
            try
            {
                var entity = await _holiday.FirstOrDefaultAsync(s => s.Id == id);
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
            var entity = await _holiday.FindAsync(id);
            if(entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            _holiday.Remove(entity);
            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
            }
        }

        public async Task Update(HolidayUpdateVModel model)
        {
            var entity = await _holiday.FindAsync(model.Id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }

            _mapper.Map(model, entity);

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedBy = GlobalUserName;

            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }
    }
}
