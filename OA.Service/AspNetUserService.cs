using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.Services.Helpers;
using OA.Domain.VModels;
using OA.Domain.VModels.Role;
using OA.Infrastructure.EF.Context;
using OA.Infrastructure.EF.Entities;
using OA.Infrastructure.SQL;
using OA.Repository;
using OA.Service.Helpers;
using System.Data;
using System.Dynamic;

namespace OA.Service
{
    public class AspNetUserService : GlobalVariables, IAspNetUserService
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly IBaseRepository<SysFile> _sysFileRepo;
        private readonly IBaseRepository<Department> _departmentService;
        private readonly ISysConfigurationService _configService;
        private readonly IJwtFactory _jwtFactory;
        private readonly IAuthMessageSender _authMessageSender;
        private readonly IMapper _mapper;
        private static string _nameService = StringConstants.ControllerName.AspNetUser;
        private IHttpContextAccessor _contextAccessor;
        private const string _roleDefault = CommonConstants.Authorize.Member;
        private static BaseConnection _dbConnectSQL = BaseConnection.Instance();
        private readonly ApplicationDbContext _dbContext;

        public AspNetUserService(RoleManager<AspNetRole> roleManager, UserManager<AspNetUser> userManager,
                ISysConfigurationService configService, IJwtFactory jwtFactory,
                IAuthMessageSender authMessageSender, IHttpContextAccessor contextAccessor,
                IMapper mapper, IBaseRepository<SysFile> sysFileRepo, IBaseRepository<Department> departmentService,
                ApplicationDbContext dbContext) : base(contextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configService = configService;
            _jwtFactory = jwtFactory;
            _authMessageSender = authMessageSender;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
            _departmentService = departmentService;
            _sysFileRepo = sysFileRepo;
        }
        #region --USERS BASIC--
        public async Task<ResponseResult> GetAll(UserFilterVModel model)
        {
            var result = new ResponseResult();
            try
            {
                var query = _userManager.Users
                                    .Where(x =>
                                        (model.IsActive == null || x.IsActive == model.IsActive) &&
                                        (string.IsNullOrEmpty(model.Email) || x.Email == model.Email) &&
                                        (string.IsNullOrEmpty(model.PhoneNumber) || x.PhoneNumber == model.PhoneNumber) &&
                                        (string.IsNullOrEmpty(model.FullName) || x.FullName == model.FullName) &&
                                        (model.Birthday == null ||
                                            (x.Birthday.Value.Year == model.Birthday.Value.Year &&
                                            x.Birthday.Value.Month == model.Birthday.Value.Month &&
                                            x.Birthday.Value.Day == model.Birthday.Value.Day)) &&
                                        (model.CreatedDate == null ||
                                            (x.CreatedDate.HasValue &&
                                            x.CreatedDate.Value.Year == model.CreatedDate.Value.Year &&
                                            x.CreatedDate.Value.Month == model.CreatedDate.Value.Month &&
                                            x.CreatedDate.Value.Day == model.CreatedDate.Value.Day)) &&
                                        (string.IsNullOrEmpty(model.CreatedBy) || x.CreatedBy == model.CreatedBy) &&
                                        (model.UpdatedDate == null ||
                                            (x.UpdatedDate.HasValue &&
                                            x.UpdatedDate.Value.Year == model.UpdatedDate.Value.Year &&
                                            x.UpdatedDate.Value.Month == model.UpdatedDate.Value.Month &&
                                            x.UpdatedDate.Value.Day == model.UpdatedDate.Value.Day)) &&
                                        (string.IsNullOrEmpty(model.UpdatedBy) || x.UpdatedBy == model.UpdatedBy)
                                    )
                                    .OrderByDescending(x => x.CreatedDate);

                var users = await query.ToListAsync();

                var userViewModels = new List<UserGetAllVModel>();

                var userLogin = await _userManager.FindByIdAsync(GlobalUserId ?? string.Empty);
                var userRoles = userLogin != null ? await _userManager.GetRolesAsync(userLogin) : new List<string>();
                var isAdministrator = userRoles.Contains(CommonConstants.Authorize.Administrator);

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    //if (!isAdministrator && roles.Contains(CommonConstants.Authorize.Administrator))
                    //{
                    //    continue;
                    //}

                    var userViewModel = _mapper.Map<AspNetUser, UserGetAllVModel>(user);
                    userViewModel.Roles = roles.ToList();
                    userViewModel.AvatarPath = user.AvatarFileId != null ? "https://localhost:44381/" + (await _sysFileRepo.GetById((int)user.AvatarFileId))?.Path : null;
                    userViewModel.DepartmentName = user.DepartmentId.HasValue
    ? (await _departmentService.GetById(user.DepartmentId.Value))?.Name ?? ""
    : "";
                    userViewModels.Add(userViewModel);
                }

                string? keyword = model.Keyword;

                var records = userViewModels
                    .Where(r => string.IsNullOrEmpty(keyword) ||
                    (r.UserName?.ToLower()?.Contains(keyword.ToLower()) == true)
                    || (r.FullName?.ToLower()?.Contains(keyword.ToLower()) == true)
                    || (r.PhoneNumber?.ToLower()?.Contains(keyword.ToLower()) == true)
                    || (r.Email?.ToLower()?.Contains(keyword.ToLower()) == true)
                    || (r.Roles != null && r.Roles.Any(role => role.ToLower().Contains(keyword))));

                var pagedRecords = records.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

                var data = new Pagination
                {
                    Records = string.IsNullOrEmpty(model.Role) ? pagedRecords : pagedRecords.Where(x => x.Roles != null && x.Roles.Contains(model.Role)).ToList(),
                    TotalRecords = string.IsNullOrEmpty(model.Role) ? records.Count() : records.Count(x => x.Roles != null && x.Roles.Contains(model.Role))
                };
                result.Data = data;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }



        public async Task<ResponseResult> GetEmployeeCountByRole()
        {
            var result = new ResponseResult();
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var roleCounts = new Dictionary<string, int>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {

                        if (roleCounts.ContainsKey(role))
                        {
                            roleCounts[role]++;
                        }
                        else
                        {

                            roleCounts[role] = 1;
                        }
                    }
                }
                var roleCountList = roleCounts.Select(role => new { Role = role.Key, Count = role.Value }).ToList();

                result.Data = roleCountList;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }




        public async Task<ResponseResult> GetEmployeeCountByDepartment()
        {
            var result = new ResponseResult();
            try
            {
                var users = await _userManager.Users.ToListAsync();

                var departmentCounts = new Dictionary<string, int>();

                foreach (var user in users)
                {

                    var department = user.DepartmentId.HasValue
                        ? (await _departmentService.GetById(user.DepartmentId.Value))?.Name
                        : "Chưa xác định";
                    if (departmentCounts.ContainsKey(department))
                    {
                        departmentCounts[department]++;
                    }
                    else
                    {
                        departmentCounts[department] = 1;
                    }
                }
                var departmentCountList = departmentCounts.Select(department => new { Department = department.Key, Count = department.Value }).ToList();
                result.Data = departmentCountList;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }


        public async Task<ResponseResult> GetEmployeeCountByAge()
        {
            var result = new ResponseResult();
            try
            {

                var users = await _userManager.Users.ToListAsync();


                int lessThan32 = 0;
                int between32And45 = 0;
                int greaterThan45 = 0;


                foreach (var user in users)
                {
                    if (user.Birthday.HasValue)
                    {
                        var birthDate = user.Birthday.Value;
                        var age = DateTime.Now.Year - birthDate.Year;


                        if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                        {
                            age--;
                        }


                        if (age < 32)
                        {
                            lessThan32++;
                        }
                        else if (age >= 32 && age <= 45)
                        {
                            between32And45++;
                        }
                        else
                        {
                            greaterThan45++;
                        }
                    }
                }


                int totalEmployees = lessThan32 + between32And45 + greaterThan45;


                var lessThan32Percentage = totalEmployees == 0 ? 0 : ((double)lessThan32 / totalEmployees) * 100;
                var between32And45Percentage = totalEmployees == 0 ? 0 : ((double)between32And45 / totalEmployees) * 100;
                var greaterThan45Percentage = totalEmployees == 0 ? 0 : ((double)greaterThan45 / totalEmployees) * 100;


                var totalPercentage = lessThan32Percentage + between32And45Percentage + greaterThan45Percentage;
                if (totalPercentage < 100)
                {
                    greaterThan45Percentage += (100 - totalPercentage);
                }


                result.Data = new
                {
                    LessThan32 = lessThan32,
                    Between32And45 = between32And45,
                    GreaterThan45 = greaterThan45,
                    LessThan32Percentage = lessThan32Percentage,
                    Between32And45Percentage = between32And45Percentage,
                    GreaterThan45Percentage = greaterThan45Percentage
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }

        public async Task<ResponseResult> GetById(string id)
        {
            var result = new ResponseResult();
            try
            {
                var entity = await _userManager.FindByIdAsync(id);
                if (entity != null)
                {
                    var entityRoles = await _userManager.GetRolesAsync(entity);
                    var model = _mapper.Map<AspNetUser, UserGetByIdVModel>(entity);
                    if (entity.AvatarFileId != null)
                    {
                        var sysFile = await _sysFileRepo.GetById((int)entity.AvatarFileId);
                        if (sysFile != null)
                        {
                            model.AvatarPath = sysFile.Path;
                        }
                    }
                    if (entityRoles != null)
                    {
                        model.Roles = (List<string>)entityRoles;
                    }
                    result.Data = model;
                }
                else
                {
                    throw new NotFoundException(MsgConstants.WarningMessages.NotFoundData);
                };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
            return result;
        }

        public async Task<ResponseResult> GetJsonUserHasFunctions(string userId)
        {
            var result = new ResponseResult();
            var entity = await _userManager.FindByIdAsync(userId);
            if (entity != null)
            {
                var roles = await _userManager.GetRolesAsync(entity);
                dynamic objResult = new ExpandoObject();
                objResult.UserId = entity.Id;
                objResult.FullName = entity.FullName;
                objResult.JsonUserHasFunctions = entity.JsonUserHasFunctions;
                if (roles.Any())
                {
                    var rolesList = new List<string>();
                    foreach (var item in roles)
                    {
                        var roleDetail = await _roleManager.FindByNameAsync(item);
                        if (roleDetail != null && !string.IsNullOrEmpty(roleDetail.JsonRoleHasFunctions))
                        {
                            rolesList.Add(roleDetail.JsonRoleHasFunctions);
                        }
                    }
                    objResult.JsonRoleHasFunctions = rolesList;
                }

                result.Data = objResult;
            }
            else
            {
                throw new BadRequestException(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Id"));
            }
            return result;
        }

        public async Task Create(UserCreateVModel model)
        {
            await CheckValidUserName(model.UserName);
            var entity = _mapper.Map<UserCreateVModel, AspNetUser>(model);
            entity.CreatedDate = DateTime.Now;
            entity.CreatedBy = GlobalUserName;


            var maxId = await _dbContext.AspNetUsers.Where(x => x.EmployeeId != null).MaxAsync(x => x.EmployeeId) ?? "CC-000";
            string lastThreeChars = maxId.Substring(maxId.Length - 3);
            int numberPart = int.Parse(lastThreeChars) + 1;
            entity.EmployeeId = $"CC-{numberPart:D3}";
            if (entity.AvatarFileId != null)
            {
                var sysFile = await _sysFileRepo.GetById((int)entity.AvatarFileId);
                if (sysFile == null)
                {
                    entity.AvatarFileId = null;
                }
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var identityResult = await _userManager.CreateAsync(entity, model.Password);
                    if (identityResult.Succeeded)
                    {
                        var user = await _userManager.FindByNameAsync(model.UserName);

                        if (user != null)
                        {
                            var addRole = await AddRoleToUser(user, _roleDefault);
                            var updateRole = new UpdateRoleModel();
                            updateRole.UserId = user.Id;
                            updateRole.AssignRoles = model.Roles;
                            await UpdateRoleForUser(updateRole);
                            var sendMail = await SendMail(user.Email ?? string.Empty, user.Id);
                            if (sendMail.Success == false)
                            {
                                throw new BadRequestException(MsgConstants.ErrorMessages.SendEmailFailed);
                            }
                            //var sendPhone = await _authMessageSender.SendSmsAsync(user.PhoneNumber, "Your account at BlueSkyTech has been created");
                            await transaction.CommitAsync();
                        }
                        else
                        {
                            throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                        }
                    }
                    else
                    {
                        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorCreate, _nameService));
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
                }
            }
        }

        public async Task Update(UserUpdateVModel model)
        {
            try
            {
                if (model.AvatarFileId != null)
                {
                    var sysFile = await _sysFileRepo.GetById((int)model.AvatarFileId);
                    if (sysFile == null)
                    {
                        model.AvatarFileId = null;
                    }
                }
                var entity = await _userManager.FindByIdAsync(model.Id);
                if (entity != null)
                {
                    var addRole = await AddRoleToUser(entity, _roleDefault);
                    var updateRole = new UpdateRoleModel();
                    updateRole.UserId = entity.Id;
                    updateRole.AssignRoles = model.Roles;
                    await UpdateRoleForUser(updateRole);
                    var existedEmail = await _userManager.FindByEmailAsync(model.Email);
                    if (existedEmail == null || entity.Email == existedEmail.Email)
                    {
                        entity = _mapper.Map(model, entity);
                        entity.UpdatedDate = DateTime.Now;
                        entity.UpdatedBy = GlobalUserName;
                        var identityResult = await _userManager.UpdateAsync(entity);
                        if (!identityResult.Succeeded)
                        {
                            throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
                        }
                    }
                    else
                    {
                        throw new ConflictException("This email is existed in the system!");
                    }
                }
                else
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, _nameService));
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
        }

        public async Task UpdateJsonUserHasFunctions(UpdatePermissionVModel model)
        {
            var entity = await _userManager.FindByIdAsync(model.UserId);
            if (entity != null)
            {
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedBy = GlobalUserName;
                entity.JsonUserHasFunctions = model.JsonUserHasFunctions;
                var identityResult = await _userManager.UpdateAsync(entity);
                if (!identityResult.Succeeded)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
                }
            }
            else
            {
                throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, _nameService));
            }
        }

        public async Task ChangeStatus(string id)
        {
            try
            {
                var entity = await _userManager.FindByIdAsync(id);
                if (entity != null)
                {
                    entity.UpdatedDate = DateTime.Now;
                    entity.UpdatedBy = GlobalUserName;
                    entity.IsActive = !entity.IsActive;
                    var identityResult = await _userManager.UpdateAsync(entity);
                    if (!identityResult.Succeeded)
                    {
                        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorChangeStatus, _nameService));
                    }
                }
                else if (entity != null)
                {
                    throw new BadRequestException(string.Format(MsgConstants.Error404Messages.ObjectIsDeleted, _nameService));
                }
                else
                {
                    throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, _nameService));
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }
        }

        public async Task Remove(string id)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await _userManager.FindByIdAsync(id);
                    if (entity == null)
                    {
                        throw new BadRequestException(string.Format(MsgConstants.WarningMessages.NotFound, _nameService));
                    }

                    var roles = await _userManager.GetRolesAsync(entity);
                    if (roles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(entity, roles);
                    }

                    var identityResult = await _userManager.DeleteAsync(entity);

                    if (!identityResult.Succeeded)
                    {
                        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorRemove, _nameService));
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
                }
            }
        }
        #endregion --END USERS BASIC--
        #region --USERS ADVANCE--
        public async Task UpdateRoleForUser(UpdateRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                if (model.AssignRoles != null && model.AssignRoles.Count() > 0)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var identityResult = await AdjustRoles(user, model.AssignRoles, currentRoles);
                    if (!identityResult.Succeeded)
                    {
                        throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorAssignRole, user.UserName));
                    }
                }
                else
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.UserMustHaveRole, user.UserName));
                }
            }
            else
            {
                throw new NotFoundException(string.Format(MsgConstants.WarningMessages.NotFound, _nameService));
            }
        }

        public async Task CheckValidUserName(string userName)
        {
            var entity = await _userManager.FindByNameAsync(userName);
            if (entity != null)
            {
                throw new BadRequestException(string.Format(MsgConstants.Error404Messages.InvalidUsername, _nameService));
            }
        }

        public async Task CheckValidEmail(string email)
        {
            var entity = await _userManager.FindByEmailAsync(email);
            if (entity != null)
            {
                throw new BadRequestException(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, "Email"));
            }
        }

        private async Task<SendMailModel> SendMailGetInfoAsync()
        {
            var result = new SendMailModel();
            var accounts = await _configService.GetByConfigTypeKey(CommonConstants.ConfigType.typeEmail, CommonConstants.ConfigType.sendMailAccount);
            var password = await _configService.GetByConfigTypeKey(CommonConstants.ConfigType.typeEmail, CommonConstants.ConfigType.sendMailPassword);
            var header = await _configService.GetByConfigTypeKey(CommonConstants.ConfigType.typeMailTemplate, CommonConstants.ConfigType.sendMailTemplate);
            var templates = await _configService.GetByConfigTypeKey(CommonConstants.ConfigType.typeMailTemplate, CommonConstants.ConfigType.sendMailHeader);
            if (accounts.Data != null && password.Data != null && header.Data != null && templates.Data != null)
            {
                result.SendMailAccountEmail = accounts.Data?.Value;
                result.SendMailAccountPassword = password.Data?.Value;
                result.SendMailTemplateConfirmAccountTitle = header.Data?.Value;
                result.SendMailTemplateConfirmAccountBody = templates.Data?.Value;
            }
            return result;
        }

        public async Task<ResponseResult> SendMail(string toEmail, string userId)
        {
            #region ---Get config email from database---
            var configInfo = await SendMailGetInfoAsync();
            var fromEmail = configInfo.SendMailAccountEmail;
            var fromPassWord = configInfo.SendMailAccountPassword;
            var sendMailTitle = configInfo.SendMailTemplateConfirmAccountTitle;
            var sendMailBody = configInfo.SendMailTemplateConfirmAccountBody;
            #endregion ---End get config email from database---

            return await _authMessageSender.SendMailAsync(fromEmail ?? string.Empty, fromPassWord ?? string.Empty, toEmail ?? string.Empty, sendMailTitle ?? string.Empty, sendMailBody ?? string.Empty);
        }
        public bool ConfirmAccount(ConfirmAccount model)
        {
            var codeActive = _contextAccessor?.HttpContext?.Session.GetString(model.UserId);
            if (codeActive != null && codeActive == model.Code)
            {
                var isConfirm = UpdateEmailConfirm(model.UserId);
                if (isConfirm)
                {
                    _contextAccessor?.HttpContext?.Session.SetString(model.UserId, string.Empty);
                }
            }
            return true;
        }

        public async Task RequestPasswordReset(RequestResetPassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new BadRequestException(string.Format(MsgConstants.WarningMessages.NotFound, _nameService));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            try
            {
                _contextAccessor.HttpContext?.Session.SetString(user.Id, token);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(Utilities.MakeExceptionMessage(ex));
            }

            var configInfo = await SendMailGetInfoAsync();
            var fromEmail = configInfo.SendMailAccountEmail;
            var fromPassWord = configInfo.SendMailAccountPassword;
            var sendMailTitle = "Request password reset!";

            // Generate the URL with token as a query parameter
            var resetLink = $"http://localhost:3000/update-password?token={Uri.EscapeDataString(token)}";
            var sendMailBody = $@"
    <p>Hello,</p>
    <p>We received a request to reset your password. Please click the link below to proceed:</p>
    <a href=""{resetLink}"" style=""color: #007bff; text-decoration: none; font-weight: bold;"">Reset Your Password</a>
    <p>If you didn’t request this, please ignore this email.</p>
    <p>Thanks,<br/>The Team NPM</p>";

            await _authMessageSender.SendMailAsync(fromEmail ?? string.Empty, fromPassWord ?? string.Empty, model.Email, sendMailTitle, sendMailBody);
        }


        public async Task ResetPassword(ResetPasswordModel model)
        {
            var result = new ResponseResult();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new BadRequestException(string.Format(MsgConstants.WarningMessages.NotFound, _nameService));
            }

            if (model.Password != model.NewPassword)
            {
                throw new BadRequestException("Password have to equal new Password");
            }

            var confirmed = ConfirmAccount(new ConfirmAccount() { Code = model.Token, UserId = user.Id });
            if (confirmed)
            {
                var updatedResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (!updatedResult.Succeeded)
                {
                    throw new BadRequestException(MsgConstants.ErrorMessages.ErrorResetPassword);
                }
            }
            else
            {
                throw new BadRequestException("Confirm account failed!");
            }
        }

        public bool UpdateEmailConfirm(string userId)
        {
            var result = false;
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                user.EmailConfirmed = true;
                user.IsActive = CommonConstants.Status.Active;
                var identityResult = _userManager.UpdateAsync(user).Result;
                if (identityResult.Succeeded)
                {
                    result = identityResult.Succeeded;
                }
            }
            return result;
        }

        public async Task ChangePassword(UserChangePasswordVModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                var identityResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!identityResult.Succeeded)
                {
                    throw new BadRequestException(string.Format(MsgConstants.ErrorMessages.ErrorUpdate, _nameService));
                }
            }
            else
            {
                throw new BadRequestException(string.Format(MsgConstants.Error404Messages.FieldIsInvalid, _nameService));
            }
        }
        #endregion --END USERS ADVANCE--
        //COMMON FOR USERS
        private async Task<IdentityResult> AdjustRoles(AspNetUser user, IEnumerable<string> assignRoles, IList<string>? currentRoles = null)
        {
            var result = new IdentityResult();

            if (currentRoles != null)
            {
                var rolesToRemove = currentRoles.Except(assignRoles);
                var rolesToAdd = assignRoles.Except(currentRoles);

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }
                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }
            }
            else
            {
                await _userManager.AddToRolesAsync(user, assignRoles);
            }
            result = await _userManager.UpdateAsync(user);
            return result;
        }

        private async Task<IdentityResult> AddRoleToUser(AspNetUser user, string roleName)
        {
            var result = new IdentityResult();
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                result = await _userManager.AddToRoleAsync(user, roleName);
            }
            return result;
        }
    }
}