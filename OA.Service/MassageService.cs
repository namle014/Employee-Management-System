using AutoMapper;
using ChatGPT.Net.DTO;
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
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;


namespace OA.Service
{
    public class MassageService : GlobalVariables, IMassageService
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<Message> _message;
        private readonly IMapper _mapper;
        private string _nameService = "Message";
        public MassageService( ApplicationDbContext dbContext, IMapper mapper, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("context");
            _message = dbContext.Set<Message>();
            _mapper = mapper;
        }

        public async Task Create(MessageCreateVModel model)
        {
            var entity = _mapper.Map<MessageCreateVModel,Message>(model);
            entity.CreatedAt = DateTime.Now;
            
            _message.Add(entity);
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
                var messageList = await _message.AsQueryable().ToListAsync();
                result.Data = messageList;
            }
            catch (Exception ex) {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }
  
    }
}
