using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<EventCategory> EventCategories => Set<EventCategory>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();
        public DbSet<RaceDay.API.Models.Route> Routes 
            => Set<RaceDay.API.Models.Route>();
        public DbSet<Weather> WeatherRecords => Set<Weather>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .HasColumnType("varchar(20)");

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(u => u.OrganisedEvents)
                .HasForeignKey(e => e.OrganizerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Route)
                .WithOne(r => r.Event)
                .HasForeignKey<RaceDay.API.Models.Route>(r => r.EventID);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrolments)
                .HasForeignKey(e => e.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrolments)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventCategory>()
                .HasIndex(ec => new { ec.EventID, ec.CategoryID })
                .IsUnique();

            modelBuilder.Entity<Weather>()
                .Property(w => w.Conditions)
                .HasMaxLength(100);
        }
    }
}
