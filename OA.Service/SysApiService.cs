using AutoMapper;
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

namespace OA.Service
{
    public class SysApiService : BaseService<SysApi, SysApiCreateVModel, SysApiUpdateVModel, SysApiGetByIdVModel, SysApiGetAllVModel, SysApiExportVModel>, ISysApiService
    {
        private readonly IBaseRepository<SysApi> _sysApiRepo;
        private readonly IMapper _mapper;

        public SysApiService(IBaseRepository<SysApi> sysApiRepo, IMapper mapper) : base(sysApiRepo, mapper)
        {
            _sysApiRepo = sysApiRepo;
            _mapper = mapper;
        }


        public async Task<ResponseResult> GetAll(FilterSysAPIVModel model)
        {
            var result = new ResponseResult();

            var data = await _sysApiRepo.GetAllPagination(
                model.PageNumber,
                model.PageSize
            );

            if (!model.IsExport)
            {
                data.Records = data.Records.Select(r => _mapper.Map<SysApi, SysApiGetAllVModel>(r));
            }

            string? keyword = model.Keyword;
            var mappedRecords = data.Records
                .Where(r => string.IsNullOrEmpty(keyword)
                || (r.ControllerName?.ToLower()?.Contains(keyword.ToLower()) == true)
                || (r.ActionName?.ToLower()?.Contains(keyword?.ToLower()) == true)
                || (r.HttpMethod?.ToLower()?.Contains(keyword?.ToLower()) == true));

            if (!model.IsDescending)
            {
                data.Records = string.IsNullOrEmpty(model.SortBy)
                    ? mappedRecords.OrderBy(r => r.Id).ToList()
                    : mappedRecords.OrderBy(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }
            else
            {
                data.Records = string.IsNullOrEmpty(model.SortBy)
                    ? mappedRecords.OrderByDescending(r => r.Id).ToList()
                    : mappedRecords.OrderByDescending(r => r.GetType().GetProperty(model.SortBy)?.GetValue(r, null)).ToList();
            }

            data.TotalRecords = data.Records.Count();
            result.Data = data;

            return result;
        }

        public async Task<ExportStream> ExportFile(FilterSysAPIVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await GetAll(model);

            if (result.Success)
            {
                var records = _mapper.Map<IEnumerable<SysApiExportVModel>>(result.Data?.Records);
                var exportData = ImportExportHelper<SysApiExportVModel>.ExportFile(exportModel, records);
                return exportData;
            }
            else
            {
                throw new BadRequestException(MsgConstants.ErrorMessages.ErrorExport);
            }
        }
    }
}
