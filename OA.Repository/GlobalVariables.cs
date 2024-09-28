using Microsoft.AspNetCore.Http;
using OA.Core.Constants;
using System.Security.Claims;
namespace OA.Repository
{
    public class GlobalVariables
    {
        private static IHttpContextAccessor? _contextAccessor;
        public GlobalVariables(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        private static ClaimsPrincipal? User => _contextAccessor?.HttpContext?.User;
        public static string? GlobalUserName => User?.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
        public static string? GlobalUserId => User?.Identity?.IsAuthenticated == true ? User.FindFirst(CommonConstants.SpecialFields.id)?.Value : "0312300123";
    }
}
