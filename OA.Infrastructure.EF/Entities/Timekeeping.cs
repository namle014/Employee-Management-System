namespace OA.Infrastructure.EF.Entities
{
    public class Timekeeping
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string CheckInIP { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool Status { get; set; }
        public string? Note { get; set; }
        public double TotalHours { get; set; }
    }
}
