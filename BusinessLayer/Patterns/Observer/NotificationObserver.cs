using System;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using System.Linq; // For Any()

namespace BusinessLayer.Patterns.Observer
{
    public class NotificationObserver : IObserver<ConsultationDto>
    {
        private readonly INotificationService _notificationService;

        public NotificationObserver(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }
        
        public async Task UpdateAsync(ConsultationDto subject, ConsultationSubject.ConsultationEvent consultationEvent)
        {
            string notificationMessage = GenerateNotificationMessage(subject, consultationEvent);
            await SendNotificationsAsync(subject, notificationMessage, consultationEvent);
        }
        
        private string GenerateNotificationMessage(ConsultationDto consultation, ConsultationSubject.ConsultationEvent eventType)
        {
            return eventType switch
            {
                ConsultationSubject.ConsultationEvent.Created => $"New consultation '{consultation.Title}' has been scheduled for {consultation.ScheduledAt:g}.",
                ConsultationSubject.ConsultationEvent.Updated => $"Consultation '{consultation.Title}' details have been updated. Check for new time: {consultation.ScheduledAt:g}.",
                ConsultationSubject.ConsultationEvent.Cancelled => $"Consultation '{consultation.Title}' scheduled for {consultation.ScheduledAt:g} has been cancelled.",
                ConsultationSubject.ConsultationEvent.Completed => $"Consultation '{consultation.Title}' on {consultation.ScheduledAt:g} has been marked as completed.",
                ConsultationSubject.ConsultationEvent.Booked => $"Consultation '{consultation.Title}' on {consultation.ScheduledAt:g} has been booked by student {consultation.StudentName}.",
                ConsultationSubject.ConsultationEvent.MaterialAdded => $"New material has been added to consultation '{consultation.Title}'.",
                _ => $"Update for consultation '{consultation.Title}' regarding {eventType}."
            };
        }
        
        private async Task SendNotificationsAsync(ConsultationDto consultation, string message, ConsultationSubject.ConsultationEvent eventType)
        {
            try
            {
                // Notify Teacher
                if (consultation.TeacherId != Guid.Empty)
                {
                    await _notificationService.CreateNotificationAsync(
                        new NotificationDto
                        {
                            UserId = consultation.TeacherId,
                            Title = $"Consultation {eventType}",
                            Message = message,
                            Type = eventType.ToString(),
                            ConsultationId = consultation.Id,
                            SentAt = DateTime.UtcNow // Ensure SentAt is set
                        });
                }
                
                // Notify Student(s) if booked or relevant
                // The current ConsultationDto has a single StudentId and StudentName from the first link.
                // If multiple students can be linked (e.g. group consultations), this needs adjustment.
                if (consultation.StudentId.HasValue && consultation.StudentId.Value != Guid.Empty)
                {
                     await _notificationService.CreateNotificationAsync(
                        new NotificationDto
                        {
                            UserId = consultation.StudentId.Value,
                            Title = $"Consultation {eventType}",
                            Message = message,
                            Type = eventType.ToString(),
                            ConsultationId = consultation.Id,
                            SentAt = DateTime.UtcNow
                        });
                }
                // If there are other participants or subscribers, notify them here.
            }
            catch (Exception ex)
            {
                // Log the exception (using ILogger is recommended)
                Console.WriteLine($"Error sending notification: {ex.Message}");
                // Depending on policy, you might re-throw or handle gracefully
            }
        }
    }
}
