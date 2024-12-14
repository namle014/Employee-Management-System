namespace OA.Infrastructure.EF.Entities
{
    public class Salary
    {
        public String Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public decimal TotalSalary {  get; set; }
        public bool? Ispaid { get; set; }
        public string? PayrollPeriod { get; set; }
    }
}
