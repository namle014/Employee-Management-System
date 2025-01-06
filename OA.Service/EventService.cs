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
    public class EventService : GlobalVariables, IEventService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Events> _event;
        private readonly IMapper _mapper;
        private readonly string _nameService = "Event";
        public EventService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
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

        public async Task<ResponseResult> StatEventByYear(int year)
        {
            var result = new ResponseResult();
            var query = _event.AsQueryable();

            var monthlyCounts1 = await query
                 .Where(x => x.StartDate.Year == year || x.EndDate.Year == year)
                 .GroupBy(x => x.StartDate.Year == year ? x.StartDate.Month : x.EndDate.Month)
                 .Select(g => new { Month = g.Key, Count = g.Count() })
                 .ToDictionaryAsync(x => x.Month, x => x.Count);

            var list1 = Enumerable.Range(1, 12)
                .Select(i => monthlyCounts1.ContainsKey(i) ? monthlyCounts1[i] : 0)
                .ToList();

            var monthlyCounts2 = await query
                 .Where(x => x.StartDate.Year == year - 1 || x.EndDate.Year == year - 1)
                 .GroupBy(x => x.StartDate.Year == year - 1 ? x.StartDate.Month : x.EndDate.Month)
                 .Select(g => new { Month = g.Key, Count = g.Count() })
                 .ToDictionaryAsync(x => x.Month, x => x.Count);

            var list2 = Enumerable.Range(1, 12)
                .Select(i => monthlyCounts2.ContainsKey(i) ? monthlyCounts2[i] : 0)
                .ToList();


            int countYear = await query.Where(x => x.StartDate.Year == year || x.EndDate.Year == year).CountAsync();
            int countPrevYear = await query.Where(x => x.StartDate.Year == year - 1 || x.EndDate.Year == year - 1).CountAsync();

            double growthRate = 0;

            if (countPrevYear > 0)
            {
                growthRate = (double)(countYear - countPrevYear) / countPrevYear * 100;
            }
            else
            {
                growthRate = countYear > 0 ? 100 : 0;
            }

            result.Data = new
            {
                EventsByMonth = list1,
                EventsByMonthPrev = list2,
                Rate = growthRate,
            };

            return result;
        }

        public async Task<ResponseResult> TotalEventsByMonth(int month, int year)
        {
            var result = new ResponseResult();

            var query = _event.AsQueryable();

            var countCurrent = await query
                 .Where(x => (x.StartDate.Year == year && x.StartDate.Month == month) || (x.EndDate.Year == year && x.EndDate.Month == month)).CountAsync();

            if (month == 1)
            {
                year--;
                month = 12;
            }
            else
            {
                month--;
            }

            var countPrev = await query
                 .Where(x => (x.StartDate.Year == year && x.StartDate.Month == month) || (x.EndDate.Year == year && x.EndDate.Month == month)).CountAsync();


            double growthRate = 0;

            if (countPrev > 0)
            {
                growthRate = (double)(countCurrent - countPrev) / countPrev * 100;
            }
            else
            {
                growthRate = countCurrent > 0 ? 100 : 0;
            }

            result.Data = new
            {
                CountEvent = countCurrent,
                Rate = growthRate,
            };

            return result;
        }

        public async Task<ResponseResult> GetAll(EventFilterVModel model)
        {
            var result = new ResponseResult();
            var query = _event.AsQueryable();

            string? keyword = model.Keyword?.ToLower();

            var records = await _event.Where(x =>
                (model.IsHoliday == null || x.IsHoliday == model.IsHoliday) &&
                ((model.StartDate == null || model.EndDate == null) ||
                    (x.StartDate >= model.StartDate && x.EndDate <= model.EndDate)) &&
                (string.IsNullOrEmpty(keyword) ||
                    x.Title.ToLower().Contains(keyword)
                )).ToListAsync();


            if (model.IsDescending == false)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderBy(r => r.StartDate).ToList()
                        : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                        ? records.OrderByDescending(r => r.StartDate).ToList()
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
