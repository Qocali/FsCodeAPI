using Domain.Entities;
using infrastructure.DAL;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DAL
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Check if the data already exists in the database
            if (context.Products.Any())
            {
                // Data already seeded
                return;
            }

            // Add your seeding logic here
            // Example:
            var entity1 = new Products { Name = "Entity 1" };
            var entity2 = new Products { Name = "Entity 2" };
            context.Products.AddRange(entity1, entity2);

            context.SaveChanges();
        }
    }
}
