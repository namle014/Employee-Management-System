namespace OA.Core.VModels
{
    public class SalaryCreateVModel
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalSalary {  get; set; }
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
        public decimal Discipline { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Insurance { get; set; }
        public decimal Reward { get; set; }
        public decimal PITax { get; set; }
        public int Timekeeping { get; set; }
        public bool IsActive { get; set; }
        public bool? Ispaid { get; set; }
        public string? PayrollPeriod {  get; set; }
    }
    public class SalaryGetByIdVModel : SalaryGetAllVModel
    {
        
    }
    public class SalaryExportVModel
    {

    }
    public class FilterSalaryVModel
    {
        public string FullName {  get; set; } = string.Empty ;
        public int? Month { get; set; }
        public int? Year { get; set; }

        public bool? IsActive { get; set; }
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
