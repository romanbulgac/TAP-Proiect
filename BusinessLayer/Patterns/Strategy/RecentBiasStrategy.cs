using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Patterns.Strategy
{
    public class RecentBiasStrategy : IRatingStrategy
    {
        public string Name => "recent";
        public string Description => "Recent bias strategy that only considers reviews from the last 3 months";
        
        public double CalculateAverageRating(IEnumerable<Review> reviews)
        {
            if (!reviews.Any())
                return 0.0;
            
            // Consider only reviews from the last 3 months
            var recentReviews = reviews.Where(r => r.ReviewDate >= DateTime.UtcNow.AddMonths(-3)).ToList();
            
            if (!recentReviews.Any())
                return reviews.Average(r => r.Rating); // If no recent reviews, fall back to overall average
                
            return recentReviews.Average(r => r.Rating);
        }
    }
}
