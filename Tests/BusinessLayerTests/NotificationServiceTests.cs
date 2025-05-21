using Xunit;
using Moq;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Implementations;
using BusinessLayer.DTOs;
using System.Linq;
using System;
using System.Collections.Generic;
using DataAccess.Models;
using AutoMapper; // Ensure this is present
using BusinessLayer.Mapping; // Ensure this is present for MappingProfile
using BusinessLayer.Interfaces; // Ensure this is present for IRealtimeNotifier
using System.Threading.Tasks; // Ensure this is present for Task

namespace Tests.BusinessLayerTests
{
    public class NotificationServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRealtimeNotifier> _mockRealtimeNotifier;

        public NotificationServiceTests()
        {
            // Setup AutoMapper instance for tests
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile()); // Your actual mapping profile
            });
            _mapper = mappingConfig.CreateMapper();
            _mockRealtimeNotifier = new Mock<IRealtimeNotifier>();
        }

        private TestDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TestDbContext(options);
        }
        
        [Fact]
        public async Task CreateNotification_AddsNotificationToDbContextAndNotifies()
        {
            // Arrange
            var dbName = $"NotificationDb_{Guid.NewGuid()}";
            var notificationDto = new NotificationDto
            {
                Title = "Test Notification",
                Message = "This is a test notification",
                UserId = Guid.NewGuid(),
                SentAt = DateTime.UtcNow,
                Type = "TestType"
            };

            using (var context = CreateContext(dbName))
            {
                var student = new Student { Id = notificationDto.UserId, Email = "test@example.com", Name = "Test", Surname = "Student" };
                context.Students.Add(student);
                await context.SaveChangesAsync();

                var service = new NotificationService(context, _mapper, _mockRealtimeNotifier.Object);
                await service.CreateNotificationAsync(notificationDto);
            }

            // Assert
            using (var context = CreateContext(dbName)) // Use a new context instance to ensure data is read from DB
            {
                // Query without global filters for verification if needed, or rely on service method
                var notification = await context.Notifications.IgnoreQueryFilters().FirstOrDefaultAsync();
                Assert.NotNull(notification);
                Assert.Equal(notificationDto.Title, notification.Title);
                Assert.Equal(notificationDto.Message, notification.Message);
                Assert.Equal(notificationDto.UserId, notification.UserId);
            }
            // Verify that the notifier was called
            _mockRealtimeNotifier.Verify(x => x.SendNotificationToUserAsync(notificationDto.UserId.ToString(), It.IsAny<NotificationDto>()), Times.Once);
        }

        [Fact]
        public async Task GetNotificationsForUser_ReturnsUserNotifications()
        {
            // Arrange
            var dbName = $"NotificationDb_{Guid.NewGuid()}";
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var user1 = new Student { Id = userId, Email = "user@example.com", Name = "User", Surname = "Test" };
                var user2 = new Student { Id = otherUserId, Email = "other@example.com", Name = "Other", Surname = "Test" };
                context.Students.AddRange(user1, user2);

                context.Notifications.AddRange(
                    new Notification { Id = Guid.NewGuid(), Title = "User Notification 1", Message = "Msg1", UserId = userId, User = user1, CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
                    new Notification { Id = Guid.NewGuid(), Title = "User Notification 2", Message = "Msg2", UserId = userId, User = user1, CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
                    new Notification { Id = Guid.NewGuid(), Title = "Other User Notification", Message = "Msg3", UserId = otherUserId, User = user2, CreatedAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<NotificationDto> result;
            using (var context = CreateContext(dbName))
            {
                var service = new NotificationService(context, _mapper, _mockRealtimeNotifier.Object);
                result = await service.GetNotificationsForUserAsync(userId);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, n => Assert.Equal(userId, n.UserId));
            Assert.Contains(result, n => n.Title == "User Notification 1");
            Assert.Contains(result, n => n.Title == "User Notification 2");
            Assert.DoesNotContain(result, n => n.Title == "Other User Notification");
        }

        [Fact]
        public async Task SendInAppNotification_CreatesNotificationAndNotifies()
        {
            // Arrange
            var dbName = $"NotificationDb_{Guid.NewGuid()}";
            var userId = Guid.NewGuid();
            var message = "Test in-app notification message";
            
            using (var context = CreateContext(dbName))
            {
                var user = new Student { Id = userId, Email = "user@example.com", Name = "User", Surname = "Test" };
                context.Students.Add(user);
                await context.SaveChangesAsync();

                var service = new NotificationService(context, _mapper, _mockRealtimeNotifier.Object);
                await service.SendInAppNotificationAsync(message, userId);
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var notification = await context.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);
                Assert.NotNull(notification);
                Assert.Equal(message, notification.Message);
                Assert.False(notification.IsRead);
            }
            _mockRealtimeNotifier.Verify(x => x.SendNotificationToUserAsync(userId.ToString(), It.IsAny<NotificationDto>()), Times.Once);
        }


        [Fact]
        public async Task MarkNotificationAsRead_UpdatesIsReadProperty()
        {
            // Arrange
            var dbName = $"NotificationDb_{Guid.NewGuid()}";
            var userId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var user = new Student { Id = userId, Email = "user@example.com", Name = "User", Surname = "Test" };
                context.Students.Add(user);
                var notification = new Notification { Id = notificationId, Title = "Test", Message = "Msg", UserId = userId, User = user, IsRead = false, SentAt = DateTime.UtcNow };
                context.Notifications.Add(notification);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var service = new NotificationService(context, _mapper, _mockRealtimeNotifier.Object);
                await service.MarkNotificationAsReadAsync(notificationId);
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                var notification = await context.Notifications.IgnoreQueryFilters().SingleAsync(n => n.Id == notificationId);
                Assert.NotNull(notification);
                Assert.True(notification.IsRead);
            }
        }
        
        [Fact]
        public async Task SendBulkNotifications_CreatesMultipleNotifications()
        {
            var dbName = $"NotificationDb_{Guid.NewGuid()}";
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var message = "Bulk message";

            using (var context = CreateContext(dbName))
            {
                foreach (var userId in userIds)
                {
                    context.Students.Add(new Student { Id = userId, Email = $"s{userId}@e.c", Name="N", Surname="S" });
                }
                await context.SaveChangesAsync();
                
                var service = new NotificationService(context, _mapper, _mockRealtimeNotifier.Object);
                await service.SendBulkNotificationsAsync(message, userIds);
            }

            using (var context = CreateContext(dbName))
            {
                var notifications = await context.Notifications.Where(n => userIds.Contains(n.UserId)).ToListAsync();
                Assert.Equal(userIds.Count, notifications.Count);
                Assert.All(notifications, n => Assert.Equal(message, n.Message));
            }
        }


        [Fact]
        public async Task DeleteNotification_PerformsSoftDelete() // Renamed and updated
        {
            // Arrange
            var dbName = $"NotificationDb_{Guid.NewGuid()}";
            var userId = Guid.NewGuid();
            var notificationId = Guid.NewGuid();

            using (var context = CreateContext(dbName))
            {
                var user = new Student { Id = userId, Email = "user@example.com", Name = "User", Surname = "Test" };
                context.Students.Add(user);
                var notification = new Notification { Id = notificationId, Title = "Test", Message = "Msg", UserId = userId, User = user, SentAt = DateTime.UtcNow };
                context.Notifications.Add(notification);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateContext(dbName))
            {
                var service = new NotificationService(context, _mapper, _mockRealtimeNotifier.Object);
                await service.DeleteNotificationAsync(notificationId);
            }

            // Assert
            using (var context = CreateContext(dbName))
            {
                // Check with IgnoreQueryFilters to find the soft-deleted entity
                var notification = await context.Notifications
                                                .IgnoreQueryFilters()
                                                .FirstOrDefaultAsync(n => n.Id == notificationId);
                Assert.NotNull(notification);
                Assert.True(notification.IsDeleted);
                Assert.NotNull(notification.DeletedAt);

                // Ensure it's not returned by a query that respects the filter (e.g., a service call)
                var service = new NotificationService(context, _mapper, _mockRealtimeNotifier.Object);
                var userNotifications = await service.GetNotificationsForUserAsync(userId);
                Assert.DoesNotContain(userNotifications, n => n.Id == notificationId);
            }
        }
    }
}
