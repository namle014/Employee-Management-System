﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
//using Twilio.TwiML.Voice;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OA.Service
{
    public class RewardService : BaseService<Reward, RewardCreateVModel, RewardUpdateVModel, RewardGetByIdVModel, RewardGetAllVModel, RewardExportVModel>, IRewardService 
    {
        private readonly IBaseRepository<Reward> _rewardRepo;
        private readonly IMapper _mapper;

        public RewardService(IBaseRepository<Reward> rewardRepo, IMapper mapper) : base(rewardRepo, mapper)
        {
            _rewardRepo = rewardRepo;
            _mapper = mapper;
        }

        public async Task<ResponseResult> Search(RewardFilterVModel model)
        {
            var result = new ResponseResult();

            string? keyword = model.Keyword?.ToLower();
            var records = await _rewardRepo.
                        Where(x =>
                            (model.IsActive == null || model.IsActive == x.IsActive) &&
                            (model.CreatedDate == null ||
                                    (x.CreatedDate.HasValue &&
                                    x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                    x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                    x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                            (string.IsNullOrEmpty(keyword) ||
                                    (x.UserId.ToLower().Contains(keyword) == true) ||
                                    (x.Note != null && x.Note.ToLower().Contains(keyword)) ||
                                    (x.Reason != null && x.Reason.ToLower().Contains(keyword))||
                                    (x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword))||
                                    (x.UpdatedBy != null && x.UpdatedBy.ToLower().Contains(keyword))));

            if (!model.IsDescending)
            {
                records = string.IsNullOrEmpty(model.SortBy)
                    ? records.OrderBy(r => r.Id).ToList()
                    : records.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                records = string.IsNullOrEmpty(model.SortBy)
                    ? records.OrderByDescending(r => r.Id).ToList()
                    : records.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            result.Data = new Pagination();

            if (!model.IsExport)
            {
                var list = new List<RewardGetAllVModel>();
                foreach (var entity in records)
                {
                    var vmodel = _mapper.Map<RewardGetAllVModel>(entity);
                    list.Add(vmodel);
                }
                var pagedRecords = list.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

                result.Data.Records = pagedRecords;
                result.Data.TotalRecords = list.Count;
            }
            else
            {
                var pagedRecords = records.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

                result.Data.Records = pagedRecords;
                result.Data.TotalRecords = records.ToList().Count;
            }

            return result;
        }

        public async Task<ExportStream> ExportFile(RewardFilterVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await Search(model);

            var records = _mapper.Map<IEnumerable<RewardExportVModel>>(result.Data?.Records);
            var exportData = ImportExportHelper<RewardExportVModel>.ExportFile(exportModel, records);
            return exportData;
        }
        public override async Task Create(RewardCreateVModel model)
        {
            var rewardCreate =  _mapper.Map<RewardCreateVModel, Reward>(model);
            rewardCreate.Date = DateTime.Now;
            var createdResult = await _rewardRepo.Create(rewardCreate);
            //await base.Create(model);

            if (!createdResult.Success)
            {
                throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, "Object"));
            }
        }
    }
}