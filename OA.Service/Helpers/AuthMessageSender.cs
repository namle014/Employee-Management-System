using Microsoft.Extensions.Options;
using OA.Core.Configurations;
using OA.Core.Models;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace OA.Service.Helpers
{
    public class AuthMessageSender : IAuthMessageSender //: IEmailSender, ISmsSender
    {
        private readonly IOptions<SMSoptions> _options;

        public AuthMessageSender(IOptions<SMSoptions> optionsAccessor)
        {
            _options = optionsAccessor;
        }
        public async Task<ResponseResult> SendMailAsync(string fromEmail, string fromPassWord, string toEmail, string sendMailTitle, string sendMailBody)
        {
            var result = new ResponseResult();
            try
            {
                if (fromEmail != null && fromPassWord != null)
                {
                    var fromAddress = new MailAddress(fromEmail, sendMailTitle);
                    var toAddress = new MailAddress(toEmail, sendMailTitle);
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = sendMailTitle,
                        Body = sendMailBody,
                        IsBodyHtml = true
                    })
                    {
                        using (var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromAddress.Address, fromPassWord)
                        })
                        {
                            await smtp.SendMailAsync(message);
                            result.Success = true;
                        }
                    }
                }
                else
                {
                    throw new BadRequestException("Invalid email or password");
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return result;
        }

        public async Task<ResponseResult> SendSmsAsync(string number, string message)
        {
            var result = new ResponseResult();
            try
            {
                // Your SMS service integration code goes here
                // Example using Twilio:
                var accountSid = _options.Value.SMSAccountIdentification;
                var authToken = _options.Value.SMSAccountPassword;
                TwilioClient.Init(accountSid, authToken);
                // Replace with actual Twilio API call
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_options.Value.SMSAccountFrom),
                    to: new PhoneNumber(number)
                );
                result.Success = true;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            return result;
        }
    }
    public interface IAuthMessageSender
    {
        Task<ResponseResult> SendMailAsync(string fromEmail, string fromPassWord, string toEmail, string sendMailTitle, string sendMailBody);
        Task<ResponseResult> SendSmsAsync(string number, string message);
    }
}
