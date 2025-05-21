using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.BusinessLayerTests
{
    /// <summary>
    /// Special test-only DbContext with no query filters or complex configurations
    /// that would interfere with testing
    /// </summary>
    public class TestDbContext : MyDbContext
    {
        public TestDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Only include basic entity relationships, no filters
            base.OnModelCreating(builder);
            
            // Remove all global query filters for testing
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                entityType.SetQueryFilter(null);
            }
        }
    }
}
