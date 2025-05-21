using System;
using System.Collections.Generic;
using BusinessLayer.Patterns.Strategy;
using DataAccess.Models;
using Xunit;

namespace Tests.BusinessLayerTests
{
    public class RatingStrategyTests
    {
        [Fact]
        public void SimpleAverageStrategy_CalculatesCorrectAverage()
        {
            // Arrange
            var strategy = new SimpleAverageStrategy();
            var reviews = new List<Review>
            {
                new Review { Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Review { Rating = 2, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Review { Rating = 5, CreatedAt = DateTime.UtcNow }
            };
            
            // Act
            var result = strategy.CalculateAverageRating(reviews);
            
            // Assert
            Assert.Equal(3.67, result, 2); // 3.67 with precision of 2 decimal places
        }

        [Fact]
        public void WeightedAverageStrategy_PrioritizesRecentReviews()
        {
            // Arrange
            var strategy = new WeightedAverageStrategy();
            var reviews = new List<Review>
            {
                new Review { Rating = 2, CreatedAt = DateTime.UtcNow.AddDays(-60) },
                new Review { Rating = 5, CreatedAt = DateTime.UtcNow }
            };
            
            // Act
            var result = strategy.CalculateAverageRating(reviews);
            
            // Assert
            Assert.True(result > 3.5); // The weighted average should be closer to 5 than to 2
        }

        [Fact]
        public void RecentBiasStrategy_IgnoresOldReviews()
        {
            // Arrange
            var strategy = new RecentBiasStrategy();
            var reviews = new List<Review>
            {
                new Review { Rating = 1, CreatedAt = DateTime.UtcNow.AddDays(-100) },
                new Review { Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-1) }
            };
            
            // Act
            var result = strategy.CalculateAverageRating(reviews);
            
            // Assert
            Assert.Equal(5, result, 0); // Should only consider the most recent review
        }
    }
}
