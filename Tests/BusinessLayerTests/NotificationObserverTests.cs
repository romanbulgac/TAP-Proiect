using System;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using BusinessLayer.Patterns.Observer;
using Moq;
using Xunit;

namespace Tests.BusinessLayerTests
{
    public class NotificationObserverTests
    {
        [Fact]
        public async Task UpdateAsync_OnConsultationCreated_SendsNotification()
        {
            // Arrange
            var mockNotificationService = new Mock<INotificationService>();
            var observer = new NotificationObserver(mockNotificationService.Object);
            
            var consultation = new ConsultationDto
            {
                Id = Guid.NewGuid(),
                Topic = "Test Consultation",
                TeacherId = Guid.NewGuid()
            };
            
            // Act
            await observer.UpdateAsync(consultation, ConsultationSubject.ConsultationEvent.Created);
            
            // Assert
            mockNotificationService.Verify(x => x.CreateNotificationAsync(
                It.Is<NotificationDto>(n => 
                    n.Title.Contains("created", StringComparison.OrdinalIgnoreCase) && 
                    n.UserId == consultation.TeacherId)), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_OnConsultationCancelled_NotifiesBothTeacherAndStudents()
        {
            // Arrange
            var mockNotificationService = new Mock<INotificationService>();
            var observer = new NotificationObserver(mockNotificationService.Object);
            
            var studentId = Guid.NewGuid();
            var consultation = new ConsultationDto
            {
                Id = Guid.NewGuid(),
                Topic = "Test Consultation",
                TeacherId = Guid.NewGuid(),
                StudentId = studentId
            };
            
            // Act
            await observer.UpdateAsync(consultation, ConsultationSubject.ConsultationEvent.Cancelled);
            
            // Assert
            // Should notify teacher
            mockNotificationService.Verify(x => x.CreateNotificationAsync(
                It.Is<NotificationDto>(n => n.UserId == consultation.TeacherId)), 
                Times.Once);
                
            // Should notify student
            mockNotificationService.Verify(x => x.CreateNotificationAsync(
                It.Is<NotificationDto>(n => n.UserId == studentId)), 
                Times.Once);
        }
    }
}
