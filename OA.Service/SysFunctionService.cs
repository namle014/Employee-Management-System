using AutoMapper;
using Microsoft.AspNetCore.Http;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static OA.Core.Constants.CommonConstants;

namespace OA.Service
{
    public class SysFunctionService : BaseService<SysFunction, SysFunctionCreateVModel, SysFunctionUpdateVModel, SysFunctionGetByIdVModel, SysFunctionGetAllVModel, SysFunctionExportVModel>, ISysFunctionService
    {
        private readonly IBaseRepository<SysFunction> _sysFunctionRepo;
        private readonly IMapper _mapper;

        public SysFunctionService(IBaseRepository<SysFunction> sysFunctionRepo, IMapper mapper) : base(sysFunctionRepo, mapper)
        {
            _sysFunctionRepo = sysFunctionRepo;
            _mapper = mapper;
        }

        public override async Task Create(SysFunctionCreateVModel model)
        {
            if (model.ParentId != null)
            {
                var parent = await _sysFunctionRepo.GetById((int)model.ParentId);
                if (parent == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, "Parent"));
                }
            }

            await base.Create(model);
        }

        public async Task<ResponseResult> GetAll(FilterSysFunctionVModel model)
        {
            var result = new ResponseResult();

            var data = await _sysFunctionRepo.GetAllPagination(
                model.PageNumber,
                model.PageSize,
                BuildPredicate(model)
            );

            var mappedRecords = data.Records.Select(r => _mapper.Map<SysFunction, SysFunctionGetAllVModel>(r));

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

        private Expression<Func<SysFunction, bool>> BuildPredicate(FilterSysFunctionVModel model)
        {
            return m =>
                (string.IsNullOrEmpty(model.Name) || m.Name.Contains(model.Name)) &&
                (string.IsNullOrEmpty(model.CreatedBy) || m.CreatedBy == model.CreatedBy) &&
                (string.IsNullOrEmpty(model.UpdatedBy) || m.CreatedBy == model.UpdatedBy) &&
                (model.IsActive == null || m.IsActive == model.IsActive);
        }

        public override async Task Update(SysFunctionUpdateVModel model)
        {
            if (model.ParentId != null)
            {
                var parent = await _sysFunctionRepo.GetById((int)model.ParentId);
                if (parent == null)
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, "Parent"));
                }

                if (model.ParentId == model.Id)
                {
                    throw new BadRequestException("ParentId have to different Id");
                }
            }

            await base.Update(model);
        }

        public async Task<ResponseResult> GetAllAsTree(FilterSysFunctionVModel model)
        {
            var result = new ResponseResult();

            var data = await _sysFunctionRepo.GetAllPagination(
                model.PageNumber,
                model.PageSize,
                BuildPredicate(model)
            );

            IEnumerable<dynamic> records = data.Records.Select(r => _mapper.Map<SysFunction, SysFunctionGetAllAsTreesVModel>(r));

            data.Records = HandleRecursive(data.Records);
            result.Data = data;

            return result;
        }

        public IEnumerable<dynamic> HandleRecursive(IEnumerable<dynamic> records)
        {
            var parentRecords = new List<SysFunctionGetAllAsTreesVModel>();
            foreach (SysFunctionGetAllAsTreesVModel item in records)
            {
                if (item.ParentId == null)
                {
                    item.Children = GetChilds((IEnumerable<dynamic>)records, item);
                    parentRecords.Add(item);
                }
            }
            return parentRecords;
        }

        public List<SysFunctionGetAllAsTreesVModel> GetChilds(IEnumerable<dynamic> nodes, SysFunctionGetAllAsTreesVModel parentNode)
        {
            var newRecords = new List<SysFunctionGetAllAsTreesVModel>();
            var childs = nodes.Where(item => item.ParentId == parentNode.Id).ToList();

            foreach (var child in childs)
            {
                child.Children = GetChilds(nodes, child);
                newRecords.Add(child);
            }

            return newRecords;
        }

        public async Task<ResponseResult> GetJsonAPIFunctionId(int id, string type)
        {
            var result = new ResponseResult();

            var entity = await _sysFunctionRepo.GetById(id);
            if (entity != null)
            {
                dynamic objResult = new ExpandoObject();
                objResult.Id = entity.Id;
                objResult.Name = entity.Name;

                switch (type.ToUpper())
                {
                    case TypeAllowFunction.CREATE:
                        objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForCreate;
                        break;
                    case TypeAllowFunction.DELETE:
                        objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForDelete;
                        break;
                    case TypeAllowFunction.EDIT:
                        objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForEdit;
                        break;
                    case TypeAllowFunction.PRINT:
                        objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForPrint;
                        break;
                    case TypeAllowFunction.VIEW:
                        objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForView;
                        break;
                    default:
                        throw new BadRequestException("Type not accept");
                }

                result.Data = objResult;
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
            return result;
        }

        public async Task UpadateJsonAPIFunctionId(UpadateJsonAPIFunctionIdVModel model)
        {
            var entity = await _sysFunctionRepo.GetById(model.Id);
            if (entity != null)
            {
                switch (model.Type.ToUpper())
                {
                    case TypeAllowFunction.CREATE:
                        entity.JsonFunctionHasApisForCreate = model.JsonAPIFunction;
                        break;
                    case TypeAllowFunction.DELETE:
                        entity.JsonFunctionHasApisForDelete = model.JsonAPIFunction;
                        break;
                    case TypeAllowFunction.EDIT:
                        entity.JsonFunctionHasApisForEdit = model.JsonAPIFunction;
                        break;
                    case TypeAllowFunction.PRINT:
                        entity.JsonFunctionHasApisForPrint = model.JsonAPIFunction;
                        break;
                    case TypeAllowFunction.VIEW:
                        entity.JsonFunctionHasApisForView = model.JsonAPIFunction;
                        break;
                    default:
                        throw new BadRequestException("Type not accept");
                }
                var identityResult = await _sysFunctionRepo.Update(entity);
                if (!identityResult.Success)
                {
                    throw new BadRequestException("Update permission fail!");
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }
    }
}
