using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.VModels
{
    public class SalaryCreateVModel
    {
        public String UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double PITax { get; set; }
    }
    public class SalaryUpdateVModel
    {
        public String Id { get; set; } = string.Empty;
    }
    public class SalaryGetAllVModel : SalaryUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class SalaryGetByIdVModel : SalaryUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class SalaryExportVModel
    {

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
