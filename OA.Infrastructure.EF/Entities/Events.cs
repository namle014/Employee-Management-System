namespace OA.Infrastructure.EF.Entities
{
    public class Events
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsHoliday { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Color { get; set; }
        public bool AllDay { get; set; }
    }
}
