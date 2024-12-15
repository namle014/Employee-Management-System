namespace OA.Infrastructure.EF.Entities
{
    public class Reward : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Reason { get; set; }
        public decimal? Money { get; set; }
        public string? Note { get; set; }
        public bool IsReceived { get; set; } = false;
    }
}
