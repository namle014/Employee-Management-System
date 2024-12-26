namespace OA.Infrastructure.EF.Entities
{
    public class ErrorReport
    {
        public int Id { get; set; }
        public string? ReportedBy { get; set; }
        public DateTime? ReportedDate { get; set; }
        public string? Type { get; set; }
        public string? TypeId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? ResolvedBy { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? ResolutionDetails { get; set; }
    }
}
