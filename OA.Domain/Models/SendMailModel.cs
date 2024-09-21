namespace OA.Core.Models
{
    public class SendMailModel
    {
        public string? SendMailAccountEmail { get; set; }
        public string? SendMailAccountPassword { get; set; }
        public string? SendMailParametersDomainApi { get; set; }
        public string? SendMailParametersLastName { get; set; }
        public string? SendMailParametersFirstName { get; set; }
        public string? SendMailTemplateConfirmAccountBody { get; set; }
        public string? SendMailTemplateConfirmAccountTitle { get; set; }
    }
}
