using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Patterns.Adapter
{
    /// <summary>
    /// Adapter that makes Mailgun provider conform to our IEmailService interface
    /// </summary>
    public class MailgunAdapter : IEmailService
    {
        private readonly MailgunProvider _mailgunProvider;
        private readonly string _defaultFromEmail;
        private readonly ILogger<MailgunAdapter> _logger;
        
        public MailgunAdapter(MailgunProvider mailgunProvider, string defaultFromEmail, ILogger<MailgunAdapter> logger)
        {
            _mailgunProvider = mailgunProvider ?? throw new ArgumentNullException(nameof(mailgunProvider));
            _defaultFromEmail = defaultFromEmail ?? throw new ArgumentNullException(nameof(defaultFromEmail));
            _logger = logger;
        }
        
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                bool success = await _mailgunProvider.DeliverMessageAsync(
                    recipient: toEmail,
                    sender: _defaultFromEmail,
                    messageSubject: subject,
                    messageBody: body
                );
                
                if (!success)
                {
                    _logger.LogWarning($"Mailgun reported failure when sending email to {toEmail}");
                    throw new Exception("Failed to send email via Mailgun");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via Mailgun");
                throw;
            }
        }
        
        public async Task SendEmailWithTemplateAsync(string toEmail, string templateName, object templateData)
        {
            try
            {
                bool success = await _mailgunProvider.DeliverTemplateMessageAsync(
                    recipient: toEmail,
                    sender: _defaultFromEmail,
                    templateName: templateName,
                    variables: templateData
                );
                
                if (!success)
                {
                    _logger.LogWarning($"Mailgun reported failure when sending templated email to {toEmail}");
                    throw new Exception("Failed to send templated email via Mailgun");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send templated email via Mailgun");
                throw;
            }
        }
        
        public bool ValidateEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
                
            // Simple regex for email validation
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
