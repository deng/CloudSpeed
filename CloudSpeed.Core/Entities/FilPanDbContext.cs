using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudSpeed.Entities
{
    public class CloudSpeedDbContext : DbContext
    {
        public CloudSpeedDbContext(DbContextOptions<CloudSpeedDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UploadLog>().HasIndex(p => p.UserId);
        }

        public DbSet<UploadLog> UploadLogs { get; set; }

        public DbSet<FileName> FileNames { get; set; }

        public DbSet<FileCid> FileCids { get; set; }

        public DbSet<FileMd5> FileMd5s { get; set; }

        public DbSet<FileJob> FileJobs { get; set; }

        public DbSet<FileDeal> FileDeals { get; set; }

        public DbSet<FileImport> FileImports { get; set; }

        public DbSet<FileGroup> FileGroups { get; set; }
    }
}
