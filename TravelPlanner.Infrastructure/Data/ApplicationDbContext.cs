using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Core.Entities;
using TravelPlanner.Core.Enums;

namespace TravelPlanner.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }
        public DbSet<UserTrip> UserTrips { get; set; }

        public DbSet<DailyBudget> DailyBudgets { get; set; }

        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<AnalyticsEvent> AnalyticsEvents { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<UserTrip>(entity =>
            {
                entity.HasKey(ut => new { ut.UserId, ut.TripId });

                entity.HasOne(ut => ut.User)
                      .WithMany(u => u.UserTrips)
                      .HasForeignKey(ut => ut.UserId);

                entity.HasOne(ut => ut.Trip)
                      .WithMany(t => t.UserTrips)
                      .HasForeignKey(ut => ut.TripId);
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Description)
                      .HasMaxLength(2000);

                entity.Property(t => t.Location)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.Category)
                      .HasMaxLength(100);

                entity.Property(t => t.ImageUrl)
                      .HasMaxLength(500);

                entity.Property(t => t.Activities)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );

                entity.Property(t => t.Tags)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );

                entity.Property(t => t.ImageUrls)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );

                entity.Property(t => t.SuitableSeasons)
                      .HasConversion(
                          v => string.Join(',', v.Select(e => e.ToString())),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(e => Enum.Parse<Season>(e))
                                .ToList()
                      );

                entity.HasMany(t => t.PointsOfInterest)
                      .WithOne()
                      .HasForeignKey(p => p.TripId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.Location);
                entity.HasIndex(t => t.Category);
                entity.HasIndex(t => t.CreatedAt);
            });

            modelBuilder.Entity<PointOfInterest>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Description)
                      .HasMaxLength(1000);

                entity.Property(p => p.Url)
                      .HasMaxLength(500);

                entity.Property(p => p.Address)
                      .HasMaxLength(300);

                entity.Property(p => p.Category)
                      .HasMaxLength(100);

                entity.Property(p => p.ImageUrl)
                      .HasMaxLength(500);

                entity.Property(p => p.Tags)
                      .HasConversion(
                          v => string.Join(',', v),
                          v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                      );

                entity.HasIndex(p => p.TripId);
                entity.HasIndex(p => p.Category);
                entity.HasIndex(p => new { p.Latitude, p.Longitude });
            });

           
            modelBuilder.Entity<Recommendation>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.RecommendationType).HasMaxLength(100);
                entity.Property(r => r.Reason).HasMaxLength(500);
                entity.Property(r => r.Metadata).HasMaxLength(1000);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Recommendations)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Trip)
                      .WithMany()
                      .HasForeignKey(r => r.TripId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(r => r.PointOfInterest)
                      .WithMany()
                      .HasForeignKey(r => r.PointOfInterestId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.PreferenceType).HasMaxLength(100);
                entity.Property(p => p.PreferenceKey).HasMaxLength(200);
                entity.Property(p => p.PreferenceValue).HasMaxLength(200);
                entity.Property(p => p.Notes).HasMaxLength(500);

                entity.HasOne(p => p.User)
                      .WithMany(u => u.UserPreferences)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            
            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.ActivityType).HasMaxLength(100);
                entity.Property(a => a.Description).HasMaxLength(500);
                entity.Property(a => a.Metadata).HasMaxLength(1000);

                entity.HasOne(a => a.User)
                      .WithMany(u => u.ActivityLogs)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            
            modelBuilder.Entity<AnalyticsEvent>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EventName).HasMaxLength(100);
                entity.Property(e => e.EventData).HasMaxLength(500);
                entity.Property(e => e.Source).HasMaxLength(100);
                entity.Property(e => e.ErrorMessage).HasMaxLength(500);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.AnalyticsEvents)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

           
            modelBuilder.Entity<SearchHistory>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Query).HasMaxLength(500);
                entity.Property(s => s.SearchType).HasMaxLength(100);
                entity.Property(s => s.LocationName).HasMaxLength(200);
                entity.Property(s => s.SearchEngine).HasMaxLength(50);

                entity.HasOne(s => s.User)
                      .WithMany(u => u.SearchHistories)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
