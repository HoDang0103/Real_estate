using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Story> Stories { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<TopUp> TopUps { get; set; }
        public DbSet<Notify> Notifies { get; set; }
        public DbSet<Package> Packages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Add your own model configurations here.
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Stories)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Story>()
            .HasOne(s => s.Catalog)
            .WithMany(c => c.Stories)
            .HasForeignKey(s => s.CatalogID)
            .OnDelete(DeleteBehavior.Restrict);

         builder.Entity<Image>()
            .HasOne(i => i.Story)
            .WithMany(s => s.Images)
            .HasForeignKey(i => i.StoryID)
            .OnDelete(DeleteBehavior.Restrict);

         builder.Entity<TopUp>()
            .HasOne(t => t.User)
            .WithMany(u => u.TopUps)
            .HasForeignKey(t => t.UserID)
            .OnDelete(DeleteBehavior.Restrict);

         builder.Entity<Notify>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifies)
            .HasForeignKey(n => n.UserID)
            .OnDelete(DeleteBehavior.Restrict);

         builder.Entity<Package>()
            .HasMany(p => p.Stories)
            .WithOne(s => s.Package)
            .HasForeignKey(s => s.PackageID)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
