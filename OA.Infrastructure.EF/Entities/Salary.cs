namespace OA.Infrastructure.EF.Entities
{
    public class Salary
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public decimal TotalSalary {  get; set; }
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
}
