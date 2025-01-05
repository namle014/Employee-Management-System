namespace OA.Infrastructure.EF.Entities
{
    public class InsuranceUser
    {
        public int Id { get; set; }
        public string InsuranceId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime? EffectiveDate {  get; set; }
        public DateTime? ExpirationDate {  get; set; }
        public int Status { get; set; }
        public decimal EmployeeContributionRate { get; set; }
    }
}
