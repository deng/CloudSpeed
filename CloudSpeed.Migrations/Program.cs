using Microsoft.EntityFrameworkCore;
using System;

namespace CloudSpeed.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dbContext = new CloudSpeedDbContextFactory().CreateDbContext(args))
            {
                dbContext.Database.Migrate();
            }
            Console.WriteLine("Done");
        }
    }
}
