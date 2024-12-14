namespace OA.Infrastructure.EF.Entities
{
    public class InsuranceUser
    {
        public string InsuranceId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime? EffectiveDate {  get; set; }
        public DateTime? Expiration {  get; set; }
        public string Status { get; set; } = string.Empty ; 
        public decimal? EmployeeContributionRate { get; set; }
        public decimal PaidInsuranceContribution { get; set; }
    }
}
