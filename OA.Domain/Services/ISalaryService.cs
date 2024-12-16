﻿using OA.Core.Models;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Core.Services
{
    public interface ISalaryService
    {
        Task Create();
        Task Update(SalaryUpdateVModel model);
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> GetAll(SalaryFilterVModel model);
        Task Remove(string id);
        Task ChangeStatus(string id);
        Task<ResponseResult> GetIncomeInMonth(int year, int month);
        Task<ResponseResult> GetYearIncome(int year);
    }
}
