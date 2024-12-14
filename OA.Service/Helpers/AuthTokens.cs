using Newtonsoft.Json;
using OA.Core.Configurations;
using OA.Core.Models;
using OA.Core.Services.Helpers;
using System.Security.Claims;
namespace OA.Service.Helpers
{
    public class AuthTokens
    {
        public static async Task<AuthVModel> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new AuthVModel()
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };
            return response;// JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
