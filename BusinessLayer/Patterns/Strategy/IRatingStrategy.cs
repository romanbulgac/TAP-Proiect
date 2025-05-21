using DataAccess.Models;
using System.Collections.Generic;

namespace BusinessLayer.Patterns.Strategy
{
    public interface IRatingStrategy
    {
        double CalculateAverageRating(IEnumerable<Review> reviews);
        string Name { get; }
        string Description { get; }
    }
}
