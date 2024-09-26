﻿using OA.Core.Constants;
using OA.Core.Models;
using OA.Infrastructure.EF.Entities;
using System;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IBaseService<TEntity, TCreateVModel, TUpdateVModel, TGetByIdVModel, TGetAllVModel> where TEntity : BaseEntity
    {
        Task<ResponseResult> GetById(int id);
        Task Create(TCreateVModel model);
        Task Update(TUpdateVModel model);
        Task ChangeStatus(int id);
        Task Remove(int id);//Remove data 
    }
}
