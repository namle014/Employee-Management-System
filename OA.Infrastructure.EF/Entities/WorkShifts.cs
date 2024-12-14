namespace OA.Infrastructure.EF.Entities
{
    public partial class WorkShifts : BaseEntity
    {
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
