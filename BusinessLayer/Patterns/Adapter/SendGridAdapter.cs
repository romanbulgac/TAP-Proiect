using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Patterns.Adapter
{
    /// <summary>
    /// Adapter that makes SendGrid service conform to our IEmailService interface
    /// </summary>
    public class SendGridAdapter : IEmailService
    {
        private readonly SendGridService _sendGridService;
        private readonly string _defaultFromEmail;
        private readonly ILogger<SendGridAdapter> _logger;
        
        public SendGridAdapter(SendGridService sendGridService, string defaultFromEmail, ILogger<SendGridAdapter> logger)
        {
            _sendGridService = sendGridService ?? throw new ArgumentNullException(nameof(sendGridService));
            _defaultFromEmail = defaultFromEmail ?? throw new ArgumentNullException(nameof(defaultFromEmail));
            _logger = logger;
        }
        
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                await _sendGridService.SendAsync(
                    to: toEmail,
                    from: _defaultFromEmail,
                    subject: subject,
                    plainTextContent: body,
                    htmlContent: body // In a real app, you'd have HTML and plain text versions
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via SendGrid");
                throw;
            }
        }
        
        public async Task SendEmailWithTemplateAsync(string toEmail, string templateName, object templateData)
        {
            try
            {
                // In a real app, you'd have a mapping from template names to SendGrid template IDs
                string templateId = $"d-{Guid.NewGuid():N}"; // Simulated template ID
                
                await _sendGridService.SendUsingTemplateAsync(
                    to: toEmail,
                    from: _defaultFromEmail,
                    templateId: templateId,
                    templateData: templateData
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send templated email via SendGrid");
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
