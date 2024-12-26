namespace OA.Infrastructure.EF.Entities
{
    public class JobHistory
    {
       
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }
        public string? JobDescription { get; set; }
        public string? SupervisorId { get; set; }
        public string? WorkLocation { get; set; }
        public string? Allowance { get; set; }
    }
}
