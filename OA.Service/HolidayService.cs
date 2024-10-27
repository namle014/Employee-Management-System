using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<ResponseResult> GetAll()
        {
            var result = new ResponseResult();
            var query = _holiday.AsQueryable();
            var holidayList = await query.ToListAsync();
            result.Data = holidayList;
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
