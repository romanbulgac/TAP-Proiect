using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Patterns.Adapter
{
    public class EmailServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        
        public EmailServiceFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }
        
        public IEmailService CreateEmailService()
        {
            // Read email provider configuration
            string provider = _configuration["Email:Provider"] ?? "SendGrid";
            string defaultFromEmail = _configuration["Email:DefaultFromAddress"] ?? "noreply@mathconsultation.com";
            
            // Create the appropriate email service based on configuration
            return provider.ToLowerInvariant() switch
            {
                "sendgrid" => CreateSendGridService(defaultFromEmail),
                "mailgun" => CreateMailgunService(defaultFromEmail),
                _ => throw new ArgumentException($"Unsupported email provider: {provider}")
            };
        }
        
        private IEmailService CreateSendGridService(string defaultFromEmail)
        {
            string apiKey = _configuration["Email:SendGrid:ApiKey"] 
                ?? throw new InvalidOperationException("SendGrid API key is not configured");
                
            var logger = _loggerFactory.CreateLogger<SendGridService>();
            var sendGridLogger = _loggerFactory.CreateLogger<SendGridAdapter>();
            
            var sendGridService = new SendGridService(apiKey, logger);
            return new SendGridAdapter(sendGridService, defaultFromEmail, sendGridLogger);
        }
        
        private IEmailService CreateMailgunService(string defaultFromEmail)
        {
            string domain = _configuration["Email:Mailgun:Domain"]
                ?? throw new InvalidOperationException("Mailgun domain is not configured");
                
            string apiKey = _configuration["Email:Mailgun:ApiKey"]
                ?? throw new InvalidOperationException("Mailgun API key is not configured");
                
            var logger = _loggerFactory.CreateLogger<MailgunProvider>();
            var mailgunLogger = _loggerFactory.CreateLogger<MailgunAdapter>();
            
            var mailgunProvider = new MailgunProvider(domain, apiKey, logger);
            return new MailgunAdapter(mailgunProvider, defaultFromEmail, mailgunLogger);
        }
    }
}
