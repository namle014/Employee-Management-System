namespace OA.Infrastructure.EF.Entities
{
    public class EmploymentContract
    {
        public virtual AspNetUser User { get; set; }
        public String Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ContractName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal BasicSalary { get; set; }
        public string? Clause { get; set; }
        public int ProbationPeriod { get; set; }
        public int WorkingHours { get; set; }
        public string TerminationClause { get; set; } = string.Empty;
        public int ContractFileId { get; set; }
        public string? TypeContract { get; set; }
        public string? ManagerId { get; set; }
        public string? Appendix { get; set; }

    }
}
