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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace OA.Service
{
    public class TransferHistoryService : GlobalVariables, ITransferHistoryService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<TransferHistory> _transferHistory;
        private readonly IMapper _mapper;
        private readonly UserManager<AspNetUser> _userManager;
        private string _nameService = "TransferHistory";


        public TransferHistoryService(ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor, UserManager<AspNetUser> userManager) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _transferHistory = dbContext.Set<TransferHistory>();
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task Create(TransferHistoryCreateVModel model)
        {
            var entity = _mapper.Map<TransferHistoryCreateVModel, TransferHistory>(model);
            entity.TransferDate = DateTime.Now;
            var query = (await _userManager.FindByIdAsync(model.EmployeeId));
            entity.FromDepartmentId = query != null ? (query?.DepartmentId == null ? 1 : (int)query.DepartmentId) : 1;

             _transferHistory.Add(entity);
            bool success = await _dbContext.SaveChangesAsync() > 0;
            if (!success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
            }
        }

        public async Task<ResponseResult> GetAll()
        {
            var result = new ResponseResult();
            try
            {
                var transferHistoryList = await _transferHistory.AsQueryable().ToListAsync();
                result.Data = transferHistoryList;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
    }
}
