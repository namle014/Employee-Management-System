﻿using OA.Core.Models;
using OA.Domain.VModels;
using System.Security.Claims;
using System.Threading.Tasks;
namespace OA.Domain.Services
{
    public interface IAuthService
    {
        Task<AuthVModel> GenerateTokenJWT(ClaimsIdentity identity, string userName);
        Task<ResponseResult> Me();
        Task<ResponseResult> Login(CredentialsVModel model);
    }
}
