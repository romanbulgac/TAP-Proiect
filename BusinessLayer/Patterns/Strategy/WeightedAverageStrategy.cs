using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Patterns.Strategy
{
    public class WeightedAverageStrategy : IRatingStrategy
    {
        public string Name => "weighted";
        public string Description => "Weighted average giving more importance to recent reviews";
        
        public double CalculateAverageRating(IEnumerable<Review> reviews)
        {
            if (!reviews.Any())
                return 0.0;
                
            // Sort reviews by date (most recent first)
            var sortedReviews = reviews.OrderByDescending(r => r.ReviewDate).ToList();
            
            double totalWeight = 0;
            double weightedSum = 0;
            
            // Calculate a weight for each review (more recent reviews have higher weight)
            for (int i = 0; i < sortedReviews.Count; i++)
            {
                // Weight decreases as we go further back in time
                double weight = 1.0 + (sortedReviews.Count - i - 1) * 0.1;
                
                weightedSum += sortedReviews[i].Rating * weight;
                totalWeight += weight;
            }
            
            return totalWeight > 0 ? weightedSum / totalWeight : 0;
        }
    }
}
