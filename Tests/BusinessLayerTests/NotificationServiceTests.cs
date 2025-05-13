using Xunit;
using Moq;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Implementations;
using BusinessLayer.DTOs;
using System.Linq;

namespace Tests.BusinessLayerTests
{
    public class NotificationServiceTests
    {
        [Fact]
        public void CreateNotification_AddsNotificationToDbContext()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase("NotificationsTestDb")
                .Options;
            using var context = new MyDbContext(options);
            var service = new NotificationService(context);

            // Act
            service.CreateNotification(new NotificationDto
            {
                Title = "Test",
                Message = "Test message"
            });

            // Assert
            Assert.Equal(1, context.Notifications.Count());
        }
    }
}
