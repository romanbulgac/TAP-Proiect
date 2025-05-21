using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Patterns.Adapter
{
    /// <summary>
    /// Adaptee class that wraps SendGrid API
    /// </summary>
    public class SendGridService
    {
        private readonly string _apiKey;
        private readonly ILogger<SendGridService> _logger;
        
        public SendGridService(string apiKey, ILogger<SendGridService> logger)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger;
        }
        
        public async Task SendAsync(string to, string from, string subject, string plainTextContent, string htmlContent)
        {
            // In a real implementation, this would use the SendGrid API client
            _logger.LogInformation($"SendGrid: Sending email to {to} from {from} with subject: {subject}");
            
            // Simulate API call delay
            await Task.Delay(100);
            
            _logger.LogInformation("SendGrid: Email sent successfully");
        }
        
        public async Task SendUsingTemplateAsync(string to, string from, string templateId, object templateData)
        {
            _logger.LogInformation($"SendGrid: Sending templated email to {to} from {from} using template {templateId}");
            
            // Simulate API call delay
            await Task.Delay(100);
            
            _logger.LogInformation("SendGrid: Templated email sent successfully");
        }
    }
}
