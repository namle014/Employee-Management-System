using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OA.Core.Constants;
using OA.Domain.VModels;
using OA.Infrastructure.EF.Entities;
using OA.Service;
using OA.Service.Helpers;

public class CustomAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomAuthorizationMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CustomAuthorizationMiddleware(RequestDelegate next, ILogger<CustomAuthorizationMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AspNetRole>>();

        var endpoint = context.GetEndpoint();
        var authorizeAttribute = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>();

        if (authorizeAttribute?.Policy == CommonConstants.Authorize.CustomAuthorization)
        {
            var userName = context.User.Identity?.Name;

            if (userName == null || context.User.Identity?.IsAuthenticated == false)
            {
                _logger.LogWarning("Unauthorized request. User not authenticated.");
                throw new UnauthorizedAccessException();
            }

            var user = await userManager.FindByNameAsync(userName) ?? new AspNetUser();

            var roles = await userManager.GetRolesAsync(user);

            var roleJsons = new List<string>();

            foreach (var roleName in roles)
            {
                var role = await roleManager.FindByNameAsync(roleName) ?? new AspNetRole();
                if (role.JsonRoleHasFunctions != null)
                {
                    roleJsons.Add(role.JsonRoleHasFunctions.ToString());
                }
            }

            var allRoles = roleJsons
                .Where(json => !string.IsNullOrWhiteSpace(json))
                .SelectMany(json =>
                {
                    return JsonConvert.DeserializeObject<List<MenuLeft>>(json) ?? new List<MenuLeft>();
                })
                .ToList();

            var mergedRoles = allRoles
            .GroupBy(role => role.Id)
            .Select(group =>
            {
                var merged = group.First();
                merged.Function = new Function
                {
                    IsAllowAll = group.Any(r => r.Function.IsAllowAll),
                    IsAllowView = group.Any(r => r.Function.IsAllowView),
                    IsAllowCreate = group.Any(r => r.Function.IsAllowCreate),
                    IsAllowEdit = group.Any(r => r.Function.IsAllowEdit),
                    IsAllowPrint = group.Any(r => r.Function.IsAllowPrint),
                    IsAllowDelete = group.Any(r => r.Function.IsAllowDelete)
                };
                return merged;
            })
            .ToList();

            var actionDescriptor = endpoint?.Metadata?.GetMetadata<ControllerActionDescriptor>();
            var controllerName = actionDescriptor?.ControllerName;
            var actionName = actionDescriptor?.ActionName;

            bool isPrintAction = actionName?.Equals("Export", StringComparison.OrdinalIgnoreCase) ?? false;

            var method = context.Request.Method;

            string action = method switch
            {
                "GET" => "View",
                "POST" => "Create",
                "PUT" => "Edit",
                "DELETE" => "Delete",
                _ => string.Empty
            };

            bool isAuthorized = mergedRoles.Any(role =>
                role.NameController != null &&
                role.NameController.Equals(controllerName, StringComparison.OrdinalIgnoreCase) &&
                ((action == "View" && role.Function.IsAllowView && isPrintAction == false) ||
                 (isPrintAction == true && role.Function.IsAllowPrint) ||
                 (action == "Create" && role.Function.IsAllowCreate) ||
                 (action == "Edit" && role.Function.IsAllowEdit) ||
                 (action == "Delete" && role.Function.IsAllowDelete))
            ); ;

            if (!isAuthorized)
            {
                _logger.LogWarning($"Forbidden: User '{userName}' does not have access to {controllerName} with action {action}.");
                throw new ForbiddenException("Forbidden: You do not have permission to access this resource.");
            }
        }

        await _next(context);
    }
}
