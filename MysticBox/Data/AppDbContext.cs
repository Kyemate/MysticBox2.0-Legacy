using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MysticBox.Models;

namespace MysticBox.Data
{
    /// <summary>
    /// The database representational model
    /// </summary>

    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    //public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<ImageModel> Images { get; set; }

        public DbSet<AudioVideoModel> Audio_Videos { get; set; }

        public DbSet<DocumentModel> Documents { get; set; }

        public DbSet<FileModel> Files { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<AppUser>().HasKey(a => a.Id);
            modelBuilder.Entity<AppUser>().HasIndex(a => a.UploadKey).IsUnique();

            modelBuilder.Entity<ImageModel>().HasIndex(a => a.UploadKey);
            modelBuilder.Entity<AudioVideoModel>().HasIndex(a => a.UploadKey);
            modelBuilder.Entity<DocumentModel>().HasIndex(a => a.UploadKey);
            modelBuilder.Entity<FileModel>().HasIndex(a => a.UploadKey);

            modelBuilder.Entity<AppUser>().Property(x => x.UserName).HasMaxLength(16);
        }
    }
}
