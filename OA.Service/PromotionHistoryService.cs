using AutoMapper;
using ChatGPT.Net.DTO.ChatGPTUnofficial;
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
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;

namespace OA.Service
{
    public class PromotionHistoryService : GlobalVariables, IPromotionHistoryService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<PromotionHistory> _promotionHistory;
        //private DbSet<BenefitType> _benefitType;
        //private DbSet<BenefitUser> _benefitUser;
        //private readonly UserManager<AspNetUser> _userManager;
        //private readonly UserManager<AspNetUser> _userManager;
        private readonly IMapper _mapper;
        string _nameService = "Benefit";

        public PromotionHistoryService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _promotionHistory = dbContext.Set<PromotionHistory>();
           
            //_userManager = userManager;
            _mapper = mapper;
            //_userManager = userManager;
            //_userManager = userManager;
        }

        public async Task<ResponseResult> GetByUserId(string id)
        {
            var result = new ResponseResult();

            var entities = await _promotionHistory
            .Where(i => i.EmployeeId == id)
            .OrderByDescending(i => i.PromotionDate) 
            .ToListAsync();

            var list = new List<GetByIdPromotionHistory>();
            foreach (var entity in entities)
            {
                var vmodel = _mapper.Map<GetByIdPromotionHistory>(entity);

                var fromRoleName = await _dbContext.AspNetRoles.Where(i => i.Id == entity.FromRoleId).Select(i => i.Name).FirstOrDefaultAsync();
                var toRoleName = await _dbContext.AspNetRoles.Where(i => i.Id == entity.ToRoleId).Select(i => i.Name).FirstOrDefaultAsync();
                if(fromRoleName != null && toRoleName != null)
                {
                    vmodel.FromRoleName = fromRoleName;
                    vmodel.ToRoleName = toRoleName;
                }
               
                list.Add(vmodel);
            }



            //if (!entities.Any())
            //{
            //    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            //}

            //var entityMapped = _mapper.Map<List<PromotionHistory>, List<GetByIdPromotionHistory>>(entities);

            result.Data = list;

            return result;
        }

        public async Task<ResponseResult> GetAll(GetAllPromotionHistory model)
        {
            var result = new ResponseResult();
            var query = _promotionHistory.AsQueryable();
            var PromotionList = await query.ToListAsync();
           

            result.Data = new Pagination();
            var list = new List<GetAllPromotionHistory>();
            foreach (var entity in PromotionList)
            {
                var vmodel = _mapper.Map<GetAllPromotionHistory>(entity);

               

                list.Add(vmodel);
            }
            //var pagedRecords = list.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            result.Data.Records = list;
            //result.Data.TotalRecords = list.Count;

            return result;
        }

        public async Task Create(CreatePromotionHistory model)
        {
            var promotionHistory = _mapper.Map<CreatePromotionHistory, PromotionHistory>(model);
            //promotionHistory.Id = await SetIdMax(model);
            promotionHistory.PromotionDate = DateTime.UtcNow;
            //promotionHistory.CreatedBy = GlobalUserName;
            //promotionHistory.IsActive = CommonConstants.Status.Active;

            _dbContext.PromotionHistory.Add(promotionHistory);

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(UpdatePromotionHistory model)
        {
            var entity = await _promotionHistory.FindAsync(model.Id);
            if (entity == null)
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            //entity.UpdatedDate = DateTime.Now;
            //entity.UpdatedBy = GlobalUserName;

            _mapper.Map(model, entity);

            bool success = await _dbContext.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
            }
        }

        public async Task<ResponseResult> GetPromotionHistoryByMonthAndYear(int year, int month)
        {
            var result = new ResponseResult();

            if (year < 1 || month < 1 || month > 12)
            {
                throw new ArgumentException("Year or month is invalid.");
            }

            int currentQuarter = (month - 1) / 3 + 1;

            int currentQuarterStartMonth = (currentQuarter - 1) * 3 + 1;
            int currentQuarterEndMonth = currentQuarter * 3;

            var currentQuarterStart = new DateTime(year, currentQuarterStartMonth, 1);
            var currentQuarterEnd = new DateTime(year, currentQuarterEndMonth, DateTime.DaysInMonth(year, currentQuarterEndMonth));

            int previousQuarter = currentQuarter - 1;
            int previousQuarterYear = year;

            if (previousQuarter == 0) 
            {
                previousQuarter = 4;
                previousQuarterYear -= 1;
            }

            int previousQuarterStartMonth = (previousQuarter - 1) * 3 + 1;
            int previousQuarterEndMonth = previousQuarter * 3;

            var previousQuarterStart = new DateTime(previousQuarterYear, previousQuarterStartMonth, 1);
            var previousQuarterEnd = new DateTime(previousQuarterYear, previousQuarterEndMonth, DateTime.DaysInMonth(previousQuarterYear, previousQuarterEndMonth));

            var promotionHistories = await _dbContext.PromotionHistory.Where(c => c.PromotionDate <= currentQuarterEnd && c.PromotionDate >= previousQuarterStart).ToListAsync();

            var promotionHistoryCurrentQuarter = promotionHistories.Count(c =>
                c.PromotionDate <= currentQuarterEnd && c.PromotionDate >= currentQuarterStart);

            var promotionHistoryPreviousQuarter = promotionHistories.Count(c =>
                c.PromotionDate >= previousQuarterStart && c.PromotionDate <= previousQuarterEnd);

            var promotionHistoryPercent = 0;
            if (promotionHistoryPreviousQuarter == 0)
            {
                promotionHistoryPercent = 100;
            }
            else
            {
                promotionHistoryPercent = (promotionHistoryCurrentQuarter - promotionHistoryPreviousQuarter) * 100 / promotionHistoryPreviousQuarter;
            }

            result.Data = new
            {
                TotalPromotionHistory = promotionHistoryCurrentQuarter,
                PromotionHistoryPercent = promotionHistoryPercent,
                
            };
            return result;
        }
    }
}
