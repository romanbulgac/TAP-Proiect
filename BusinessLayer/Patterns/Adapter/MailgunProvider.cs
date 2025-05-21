using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Patterns.Adapter
{
    /// <summary>
    /// Adaptee class that wraps Mailgun API
    /// </summary>
    public class MailgunProvider
    {
        private readonly string _domain;
        private readonly string _apiKey;
        private readonly ILogger<MailgunProvider> _logger;
        
        public MailgunProvider(string domain, string apiKey, ILogger<MailgunProvider> logger)
        {
            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger;
        }
        
        public async Task<bool> DeliverMessageAsync(string recipient, string sender, string messageSubject, string messageBody)
        {
            // In a real implementation, this would use the Mailgun API client
            _logger.LogInformation($"Mailgun: Delivering message to {recipient} from {sender} with subject: {messageSubject}");
            
            // Simulate API call delay
            await Task.Delay(100);
            
            _logger.LogInformation("Mailgun: Message delivered successfully");
            return true;
        }
        
        public async Task<bool> DeliverTemplateMessageAsync(string recipient, string sender, string templateName, object variables)
        {
            _logger.LogInformation($"Mailgun: Delivering templated message to {recipient} from {sender} using template {templateName}");
            
            // Simulate API call delay
            await Task.Delay(100);
            
            _logger.LogInformation("Mailgun: Templated message delivered successfully");
            return true;
        }
    }
}
