﻿using OA.Core.Models;
using OA.Core.VModels;
using OA.Infrastructure.EF.Entities;

namespace OA.Core.Services
{
    public interface ISalaryService
    {
        Task Create();
        Task Update(string Id);
        Task<ResponseResult> GetById(string id);
        Task<ResponseResult> GetAll(SalaryFilterVModel model, string period);
        Task Remove(string id);
        Task PaymentConfirmation(string Id);
        Task ChangeStatus(string id);
        Task<ResponseResult> GetIncomeInMonth(int year, int month);
        Task<ResponseResult> GetYearIncome(int year);
        Task ChangeStatusMany(SalaryChangeStatusManyVModel model);
        Task<ResponseResult> GetInfoForDepartmentChart();
        Task<ResponseResult> GetSalaryByLevel();
        Task<ResponseResult> GetInfoForSalarySummary();
        Task<ResponseResult> GetTotalIncomeOverTime();
        Task<ResponseResult> GetIncomeStructure();
        Task<ResponseResult> GetPeriod();
        Task<ResponseResult> GetTotalBySex();
        Task<ResponseResult> GetGrossTotal();
        Task<ResponseResult> GetTotalMaxMin();
        Task<ResponseResult> GetDisplayInfo();
        Task<ResponseResult> GetGrossTotalByDepartments();
        Task<ResponseResult> GetPayrollOfDepartmentOvertime(int year);
        Task<ResponseResult> GetPayrollReport(int year);
        Task<ResponseResult> PayrollOverview(string period);
        Task<ResponseResult> GetUnpaidSalary(SalaryFilterVModel model, int year);
        Task<ResponseResult> GetMeInfo();
        Task<ResponseResult> GetMeInfoCycle(int year);
        Task<ResponseResult> GetIncomeByYear(int year);
        Task<ResponseResult> GetMeSalaryInfo(string id);
    }
}
