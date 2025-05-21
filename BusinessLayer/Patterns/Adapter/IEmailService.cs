using System.Threading.Tasks;

namespace BusinessLayer.Patterns.Adapter
{
    /// <summary>
    /// Target interface for the Adapter pattern
    /// Defines a common interface for all email service providers
    /// </summary>
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailWithTemplateAsync(string toEmail, string templateName, object templateData);
        bool ValidateEmailAddress(string email);
    }
}
