using AutoMapper;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service.Helpers;

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

            string? keyword = model.Keyword?.ToLower();

            var records = await _sysFunctionRepo.Where(x =>
                        (model.IsActive == null || x.IsActive == model.IsActive) &&
                        (model.CreatedDate == null ||
                                (x.CreatedDate.HasValue &&
                                x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                        (string.IsNullOrEmpty(keyword) ||
                                x.Name.ToLower().Contains(keyword) ||
                                x.CreatedBy != null && x.CreatedBy.ToLower().Contains(keyword)
                        ));

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

            var mappedRecords = records.Select(r => _mapper.Map<SysFunction, SysFunctionGetAllVModel>(r));

            var pagedRecords = mappedRecords.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

            result.Data.Records = mappedRecords;
            result.Data.TotalRecords = records.ToList().Count;

            return result;
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

            string? keyword = model.Keyword?.ToLower();

            var records = await _sysFunctionRepo.Where(x =>
                        (model.IsActive == null || x.IsActive == model.IsActive) &&
                        (model.CreatedDate == null ||
                                (x.CreatedDate.HasValue &&
                                x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                        (string.IsNullOrEmpty(keyword) ||
                                x.Name.ToLower().Contains(keyword)
                        ));

            result.Data = new Pagination();

            var mappedRecords = records.Select(r => _mapper.Map<SysFunction, SysFunctionGetAllAsTreesVModel>(r));

            var solveRecords = HandleRecursive(mappedRecords);

            var pagedRecords = solveRecords.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

            result.Data.Records = pagedRecords;
            result.Data.TotalRecords = solveRecords.ToList().Count;

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

        //public async Task<ResponseResult> GetJsonAPIFunctionId(int id, string type)
        //{
        //    var result = new ResponseResult();

        //    var entity = await _sysFunctionRepo.GetById(id);
        //    if (entity != null)
        //    {
        //        dynamic objResult = new ExpandoObject();
        //        objResult.Id = entity.Id;
        //        objResult.Name = entity.Name;

        //        switch (type.ToUpper())
        //        {
        //            case TypeAllowFunction.CREATE:
        //                objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForCreate;
        //                break;
        //            case TypeAllowFunction.DELETE:
        //                objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForDelete;
        //                break;
        //            case TypeAllowFunction.EDIT:
        //                objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForEdit;
        //                break;
        //            case TypeAllowFunction.PRINT:
        //                objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForPrint;
        //                break;
        //            case TypeAllowFunction.VIEW:
        //                objResult.JsonAPIFunctions = entity.JsonFunctionHasApisForView;
        //                break;
        //            default:
        //                throw new BadRequestException("Type not accept");
        //        }

        //        result.Data = objResult;
        //    }
        //    else
        //    {
        //        throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
        //    }
        //    return result;
        //}

        //public async Task UpadateJsonAPIFunctionId(UpadateJsonAPIFunctionIdVModel model)
        //{
        //    var entity = await _sysFunctionRepo.GetById(model.Id);
        //    if (entity != null)
        //    {
        //        switch (model.Type.ToUpper())
        //        {
        //            case TypeAllowFunction.CREATE:
        //                entity.JsonFunctionHasApisForCreate = model.JsonAPIFunction;
        //                break;
        //            case TypeAllowFunction.DELETE:
        //                entity.JsonFunctionHasApisForDelete = model.JsonAPIFunction;
        //                break;
        //            case TypeAllowFunction.EDIT:
        //                entity.JsonFunctionHasApisForEdit = model.JsonAPIFunction;
        //                break;
        //            case TypeAllowFunction.PRINT:
        //                entity.JsonFunctionHasApisForPrint = model.JsonAPIFunction;
        //                break;
        //            case TypeAllowFunction.VIEW:
        //                entity.JsonFunctionHasApisForView = model.JsonAPIFunction;
        //                break;
        //            default:
        //                throw new BadRequestException("Type not accept");
        //        }
        //        var identityResult = await _sysFunctionRepo.Update(entity);
        //        if (!identityResult.Success)
        //        {
        //            throw new BadRequestException("Update permission fail!");
        //        }
        //    }
        //    else
        //    {
        //        throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
        //    }
        //}
    }
}
