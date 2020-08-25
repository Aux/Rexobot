using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Rexobot
{
    public class RootDatabase : DbContext
    {
        public DbSet<RexoUser> Users { get; set; }
        public DbSet<RexoProduct> Products { get; set; }
        public DbSet<RexoSyncedRole> SyncedRoles { get; set; }

        public RootDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "common/data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string dataDir = Path.Combine(baseDir, "root.sqlite.db");
            builder.UseSqlite($"Filename={dataDir}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RexoUser>()
                .HasKey(x => x.Id);
            builder.Entity<RexoUser>()
                .HasMany(x => x.Roles)
                .WithOne(x => x.User);

            builder.Entity<RexoProduct>()
                .HasKey(x => x.Id);
            builder.Entity<RexoProduct>()
                .HasMany(x => x.Roles)
                .WithOne(x => x.Product);
        }
    }
}
