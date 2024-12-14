using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OA.Core.Configurations;
using OA.Core.Constants;
using OA.Core.Models;
using OA.Core.Repositories;
using OA.Core.Services;
using OA.Core.Services.Helpers;
using OA.Domain.Services;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Repository;
using OA.Service.Helpers;
using System.Security.Claims;

namespace OA.Service
{
    public class AuthService : GlobalVariables, IAuthService
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IAspNetUserService _userService;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<SysFile> _sysFileRepo;

        public AuthService(UserManager<AspNetUser> userManager, IAspNetUserService userService, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IHttpContextAccessor contextAccessor, IBaseRepository<SysFile> sysFileRepo, IMapper mapper) : base(contextAccessor)
        {
            _userManager = userManager;
            _userService = userService;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _sysFileRepo = sysFileRepo;
            _mapper = mapper;
        }
        public async Task<AuthVModel> GenerateTokenJWT(ClaimsIdentity identity, string userName)
        {
            if (identity == null) return new AuthVModel();
            DateTime tokenIssuedTime = new DateTime();
            var jwt = new AuthVModel();
            if (DateTime.Now > tokenIssuedTime.Add(_jwtOptions.ValidFor))
            {
                jwt = await AuthTokens.GenerateJwt(identity, _jwtFactory, userName, _jwtOptions
                            , new JsonSerializerSettings { Formatting = Formatting.Indented });

                _jwtOptions.ValidFor = TimeSpan.FromMinutes(60);
            }
            return jwt;
        }

        List<MenuLeft>? CreateMenuLeft(string jsonUserHasFunctions)
        {
            if (jsonUserHasFunctions != null)
            {
                var menus = JsonConvert.DeserializeObject<List<MenuLeft>>(jsonUserHasFunctions);

                void AddChildMenus(MenuLeft parentMenu)
                {
                    var childMenus = menus.Where(menu => menu.ParentId == parentMenu.Id).ToList();

                    foreach (var childMenu in childMenus)
                    {
                        AddChildMenus(childMenu);

                        parentMenu.Childs.Add(childMenu);
                    }
                }

                if (menus == null)
                {
                    return null;
                }

                foreach (var menu in menus)
                {
                    if (menu.ParentId == null)
                    {
                        AddChildMenus(menu);
                    }
                }
                var menuLeft = menus.Where(menu => menu.ParentId == null).ToList();
                return menuLeft;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseResult> Login(CredentialsVModel credentials)
        {
            var result = new ResponseResult();
            if (string.IsNullOrEmpty(credentials.Email) || string.IsNullOrEmpty(credentials.Password))
            {
                throw new BadRequestException(MsgConstants.Error404Messages.InvalidUsernameOrPassword);
            }

            var user = await _userManager.FindByEmailAsync(credentials.Email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(credentials.Email);
            }
            if (user != null && user.IsActive == CommonConstants.Status.Active)
            {
                if (await _userManager.CheckPasswordAsync(user, credentials.Password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var identity = await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(credentials.Email, user.Id, roles.ToList()));
                    result.Data = await GenerateTokenJWT(identity, credentials.Email);
                }
                else
                {
                    throw new BadRequestException(MsgConstants.Error404Messages.InvalidPassword);
                }
            }
            else if (user != null && user.IsActive == CommonConstants.Status.InActive)
            {
                throw new BadRequestException(CommonConstants.AccountStatus.MsgInActive);
            }
            else
            {
                throw new BadRequestException(MsgConstants.Error404Messages.InvalidUsername);
            }
            return result;
        }

        public async Task<ResponseResult> Me()
        {
            var result = new ResponseResult();

            var entity = await _userManager.FindByIdAsync(GlobalUserId ?? string.Empty);
            if (entity == null)
            {
                throw new BadRequestException(MsgConstants.WarningMessages.NotFoundData);
            }

            var entityRoles = await _userManager.GetRolesAsync(entity);
            var model = _mapper.Map<AspNetUser, GetMeVModel>(entity);

            if (entity.AvatarFileId != null)
            {
                var entityFile = await _sysFileRepo.GetById((int)entity.AvatarFileId);
                if (entityFile != null)
                {
                    model.AvatarPath = entityFile.Path;
                }
            }

            if (entityRoles != null)
            {
                model.Roles = (List<string>)entityRoles;
            }

            model.MenuLeft = CreateMenuLeft(entity.JsonUserHasFunctions) ?? new List<MenuLeft>();

            result.Data = model;
            return result;
        }
    }
}
