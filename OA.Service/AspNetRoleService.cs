using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.VModels;
using OA.Domain.VModels.Role;
using OA.Infrastructure.EF.Entities;
using OA.Infrastructure.SQL;
using OA.Repository;
using OA.Service.Helpers;
using System.Dynamic;
using static OA.Core.Constants.MsgConstants;

namespace OA.Service
{
    public class AspNetRoleService : GlobalVariables, IAspNetRoleService
    {
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly UserManager<AspNetUser> _userManager;
        private static string _nameService = StringConstants.ControllerName.AspNetRole;
        private readonly IMapper _mapper;

        private static BaseConnection _dbConnectSQL = BaseConnection.Instance();
        public AspNetRoleService(RoleManager<AspNetRole> roleManager,
            UserManager<AspNetUser> userManager, IHttpContextAccessor contextAccessor, IMapper mapper, IBaseRepository<SysFunction> sysFunctionRepo) : base(contextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<ResponseResult> GetById(string id)
        {
            var result = new ResponseResult();
            var entity = await _roleManager.FindByIdAsync(id) ?? throw new NotFoundException(WarningMessages.NotFoundData);
            var roleGetByIdVModel = _mapper.Map<AspNetRoleGetByIdVModel>(entity);
            result.Data = roleGetByIdVModel;
            return result;
        }

        public Task<ResponseResult> GetAll(FiltersGetAllByQueryStringRoleVModel model)
        {
            var result = new ResponseResult();
            var query = _roleManager.Roles.AsQueryable().ToList()
                        .OrderByDescending(x => x.CreatedDate);
            var data = new Pagination
            {
                Records = query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize),
                TotalRecords = query.Count()
            };
            data.Records = data.Records.Select(r => Utilities.ConvertModel<AspNetRoleGetAllVModel>(r));
            result.Data = data;

            return Task.FromResult(result);
        }

        public async Task CheckValidRoleName(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                throw new BadRequestException(string.Format(MsgConstants.Error404Messages.ErrorCheckValidRoleName, "RoleName"));
            }
        }

        public async Task<ResponseResult> GetJsonHasFunctionByRoleId(string id)
        {
            var result = new ResponseResult();
            var entity = await _roleManager.FindByIdAsync(id) ?? throw new NotFoundException(WarningMessages.NotFoundData);
            dynamic objResult = new ExpandoObject();
            objResult.RoleId = entity.Id;
            objResult.Name = entity.Name;
            objResult.JsonRoleHasFunctions = entity.JsonRoleHasFunctions;
            result.Data = objResult;

            return result;
        }
        public async Task Create(AspNetRoleCreateVModel model)
        {
            var roleExist = await _roleManager.RoleExistsAsync(model.Name);
            if (!roleExist)
            {
                var entityCreated = _mapper.Map<AspNetRole>(model);
                entityCreated.NormalizedName = _roleManager.NormalizeKey(model.Name);
                entityCreated.CreatedDate = DateTime.Now;
                entityCreated.CreatedBy = GlobalUserName ?? null;
                await _roleManager.CreateAsync(entityCreated);
            }
            else
            {
                throw new ConflictException(string.Format(MsgConstants.Existed.ObjectIsExisted, _nameService));
            }
        }

        public async Task Update(AspNetRoleUpdateVModel model)
        {
            var entity = await _roleManager.FindByIdAsync(model.Id);
            if (entity != null)
            {
                entity.Name = model.Name;
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedBy = GlobalUserName;
                entity.NormalizedName = _roleManager.NormalizeKey(entity.Name);
                entity.IsActive = model.IsActive;
                await _roleManager.UpdateAsync(entity);
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public async Task UpadateJsonHasFunctionByRoleId(UpadateJsonHasFunctionByRoleIdVModel model)
        {
            var entity = await _roleManager.FindByIdAsync(model.Id);
            if (entity != null)
            {
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedBy = GlobalUserName;
                entity.JsonRoleHasFunctions = model.JsonRoleHasFunctions;
                var updateResult = await _roleManager.UpdateAsync(entity);
                if (!updateResult.Succeeded)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public async Task ChangeStatus(string id)
        {
            var items = await _roleManager.FindByIdAsync(id);
            if (items != null)
            {
                items.UpdatedDate = DateTime.Now;
                items.UpdatedBy = GlobalUserName;
                items.IsActive = !items.IsActive;
                await _roleManager.UpdateAsync(items);
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public async Task Remove(string id)
        {
            var result = new ResponseResult();
            var entity = await _roleManager.FindByIdAsync(id);
            if (entity != null)
            {
                var userRoles = await _userManager.GetUsersInRoleAsync(entity.Name ?? string.Empty);
                if (userRoles.Any())
                {
                    foreach (var user in userRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, entity.Name ?? string.Empty);
                    }
                }
                var deleteResult = await _roleManager.DeleteAsync(entity);
                if (!deleteResult.Succeeded)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
                }
            }
            else
            {
                throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
            }
        }

        public virtual async Task<ExportStream> ExportFile(FiltersGetAllByQueryStringRoleVModel model, ExportFileVModel exportModel)
        {
            model.IsExport = true;
            var result = await GetAll(model);

            var records = _mapper.Map<IEnumerable<AspNetRoleExport>>(result.Data?.Records);
            var exportData = ImportExportHelper<AspNetRoleExport>.ExportFile(exportModel, records);
            return await Task.FromResult(exportData);
        }
    }

}
