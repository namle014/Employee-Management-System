﻿using OA.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace OA.Core.VModels
{
    public class SalaryCreateVModel
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
    public class SalaryUpdateVModel
    {
        public string Id { get; set; } = string.Empty;
        public decimal TotalSalary { get; set; }
    }
    public class SalaryGetAllVModel : SalaryCreateVModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal Benefit {  get; set; }
        public decimal? Discipline { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Insurance { get; set; }
        public decimal? Reward { get; set; }
        public decimal PITax { get; set; }
        public double? Timekeeping { get; set; }
        public bool IsActive { get; set; }
        public bool? Ispaid { get; set; }
        public string PayrollPeriod { get; set; } = string.Empty;
        public decimal TotalSalary { get; set; }
    }

    public class UnPaidSalaryVModel : SalaryGetAllVModel
    {
        public string? AvatarPath { get; set; }

    }

    public class MyInfo
    {
        public string? AvatarPath { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public List<string> RoleName { get; set; } = new List<string>();
        public string DepartmentName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? StartDateWork { get; set; }
        public int? PayrollCycle { get; set; }
    }
    public class MyInfoCycleVModel()
    {
        public string Id { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public bool Ispaid { get; set; }
        public double? NumberOfWorkingHours { get; set; }
        public decimal TotalSalary { get; set; }
    }
    public class SalaryGetByIdVModel
    {
        public string? AvatarPath { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? EmployeeId { get; set; }
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<string> RoleName { get; set; } = new List<string>();
        public string DepartmentName { get; set; } = string.Empty;
        public string? Date { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal SalaryPayment { get; set; }
        public bool? IsPaid { get; set; }
        public string PayrollPeriod { get; set; } = string.Empty;
        public decimal ProRatedSalary { get; set; }
        public decimal PITax { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal TotalBenefit { get; set; }
        public decimal? TotalReward { get; set; }
        public decimal? TotalDiscipline { get; set; }
        public double? NumberOfWorkingHours { get; set; }
    }
    public class SalaryExportVModel
    {

    }

    public class SalaryChangeStatusManyVModel
    {
        public List<string> Ids { get; set; } = new List<string>();
    }

    public class SalaryFilterVModel
    {
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedDate { get; set; }
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = CommonConstants.ConfigNumber.pageSizeDefault;
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public bool IsExport { get; set; } = false;
        public bool IsDescending { get; set; } = true;
        public string? Keyword { get; set; }
    }
    public class TotalIncomeVmodel
    {
        public string payrollPeriod { get; set; } = string.Empty;
        public decimal TotalIncome { get; set; }
        public decimal TotalSalary { get; set; }
    }
    //public class UserVModel
    //{
    //    public string FirstName { get; set; } 
    //    public string LastName { get; set; }
    //    [RegularExpression("^[a-zA-Z0-9]*$")]
    //    public string UserName { get; set; }
    //    [DataType(DataType.EmailAddress)]
    //    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    //    public string Email { get; set; }
    //    [RegularExpression("^[0-9]*$")]
    //    public string? PhoneNumber { get; set; }
    //    public int? Sex { get; set; }
    //    public string Address { get; set; }
    //    public DateTime? Birthday { get; set; }
    //}
    //public class SalaryVModel : UserVModel
    //{
    //    public double BasicSalary { get; set; }
    //    public double Allowance { get; set; }
    //    public double Reward { get; set; }
    //    public double Discipline {  get; set; }
    //    public double PITax { get; set; }
    //    public double TotalSalary { get; set; } = 0.0;
    //}

}
