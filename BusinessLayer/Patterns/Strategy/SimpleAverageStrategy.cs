using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Patterns.Strategy
{
    public class SimpleAverageStrategy : IRatingStrategy
    {
        public string Name => "simple";
        public string Description => "Simple arithmetic average of all ratings";
        
        public double CalculateAverageRating(IEnumerable<Review> reviews)
        {
            if (!reviews.Any())
                return 0.0;
                
            return reviews.Average(r => r.Rating);
        }
    }
}
