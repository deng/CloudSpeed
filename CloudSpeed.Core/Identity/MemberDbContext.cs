using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudSpeed.Identity
{
    public class MemberDbContext : IdentityDbContext<Member>
    {
        public MemberDbContext(DbContextOptions<MemberDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Member>().HasIndex(p => p.Created);
        }
    }
}
