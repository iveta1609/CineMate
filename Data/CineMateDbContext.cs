using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CineMate.Data.Entities;
using CineMate.Data.Entities.CineMate.Data.Entities;

namespace CineMate.Data
{
    public class CineMateDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public CineMateDbContext(DbContextOptions<CineMateDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Cinema> Cinemas { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Screening> Screenings { get; set; } = null!;
        public DbSet<Seat> Seats { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<ReservationSeat> ReservationSeats { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed роли
            var adminRoleId = "a1111111-aaaa-4aaa-aaaa-aaaaaaaaaaa1";
            var operatorRoleId = "b2222222-bbbb-4bbb-bbbb-bbbbbbbbbbb2";
            var clientRoleId = "c3333333-cccc-4ccc-cccc-ccccccccccc3";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                new IdentityRole { Id = operatorRoleId, Name = "Operator", NormalizedName = "OPERATOR" },
                new IdentityRole { Id = clientRoleId, Name = "Client", NormalizedName = "CLIENT" }
            );

            // Seed админ и оператор и демо клиент
            var adminUserId = "e5555555-eeee-4eee-eeee-eeeeeeeeeee5";
            var operatorUserId = "f6666666-ffff-4fff-ffff-fffffffffff6";
            var demoUserId = "d4444444-dddd-4ddd-dddd-dddddddddddd";

            var hasher = new PasswordHasher<IdentityUser>();

            modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = adminUserId,
                    UserName = "admin@cinemate.local",
                    NormalizedUserName = "ADMIN@CINEMATE.LOCAL",
                    Email = "admin@cinemate.local",
                    NormalizedEmail = "ADMIN@CINEMATE.LOCAL",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin123!"),
                },
                new IdentityUser
                {
                    Id = operatorUserId,
                    UserName = "operator@cinemate.local",
                    NormalizedUserName = "OPERATOR@CINEMATE.LOCAL",
                    Email = "operator@cinemate.local",
                    NormalizedEmail = "OPERATOR@CINEMATE.LOCAL",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Operator123!"),
                },
                new IdentityUser
                {
                    Id = demoUserId,
                    UserName = "demo@cinemate.local",
                    NormalizedUserName = "DEMO@CINEMATE.LOCAL",
                    Email = "demo@cinemate.local",
                    NormalizedEmail = "DEMO@CINEMATE.LOCAL",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Password123!"),
                }
            );

            // Присвояване роли
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = operatorUserId, RoleId = operatorRoleId },
                new IdentityUserRole<string> { UserId = demoUserId, RoleId = clientRoleId }
            );

            // 4) Seed Cities → Cinemas
            modelBuilder.Entity<City>().HasData(
              new City { Id = -1, Name = "Sofia" },
              new City { Id = -2, Name = "Plovdiv" },
              new City { Id = -3, Name = "Varna" }
            );
            modelBuilder.Entity<Cinema>().HasData(
               new Cinema { Id = -1, Name = "CineMate Sofia Central", CityId = -1 },
               new Cinema { Id = -2, Name = "CineMate Mall of Sofia", CityId = -1 },
               new Cinema { Id = -3, Name = "CineMate Paradise", CityId = -1 },
                new Cinema { Id = -4, Name = "CineMate Arena Mladost", CityId = -1 },
                 new Cinema { Id = -5, Name = "CineMate The Mall", CityId = -1 },
                new Cinema { Id = -6, Name = "CineMate Plovdiv Mall", CityId = -2 },
                new Cinema { Id = -7, Name = "CineMate Central Park", CityId = -2 },
                new Cinema { Id = -8, Name = "CineMate Mall Varna", CityId = -3 },
                new Cinema { Id = -9, Name = "CineMate Sea Garden", CityId = -3 }
            );

            // Seed филми (11)
            modelBuilder.Entity<Movie>().HasData(
                new Movie { Id = 1, Title = "F1", Director = "TBA", ReleaseYear = 2025, Genre = "Sports Drama", Synopsis = "A behind-the-scenes look at the rivalries and politics of Formula 1.", DurationMinutes = 120 },
                new Movie { Id = 2, Title = "Jurassic World: Rebirth", Director = "Colin Trevorrow", ReleaseYear = 2025, Genre = "Adventure / Sci-Fi", Synopsis = "Scientists’ attempt to fully resurrect dinosaurs leads to catastrophic consequences.", DurationMinutes = 140 },
                new Movie { Id = 3, Title = "Superman (2025)", Director = "James Gunn", ReleaseYear = 2025, Genre = "Superhero / Action", Synopsis = "Clark Kent defends Metropolis from a mysterious Kryptonian threat.", DurationMinutes = 130 },
                new Movie { Id = 4, Title = "The Fantastic Four: First Steps", Director = "Jonas Reiss", ReleaseYear = 2025, Genre = "Superhero / Adventure", Synopsis = "A young team of scientists gains cosmic powers after a space radiation accident.", DurationMinutes = 125 },
                new Movie { Id = 5, Title = "28 Years Later", Director = "Juan Carlos Fresnadillo", ReleaseYear = 2025, Genre = "Horror / Post-Apocalyptic", Synopsis = "Decades after the initial outbreak, survivors struggle against a new viral strain.", DurationMinutes = 112 },
                new Movie { Id = 6, Title = "Now You See Me: Now You Don't", Director = "Louis Leterrier", ReleaseYear = 2025, Genre = "Crime Thriller", Synopsis = "The Four Horsemen return to execute an even more daring heist under the FBI’s nose.", DurationMinutes = 110 },
                new Movie { Id = 7, Title = "Mission: Impossible – Retribution", Director = "Christopher McQuarrie", ReleaseYear = 2025, Genre = "Action / Spy", Synopsis = "Ethan Hunt uncovers an international conspiracy threatening his team’s safety.", DurationMinutes = 147 },
                new Movie { Id = 8, Title = "Sinners", Director = "Sarah Johnson", ReleaseYear = 2024, Genre = "Psychological Thriller", Synopsis = "A therapist confronts her own dark past when treating a mysterious new patient.", DurationMinutes = 98 },
                new Movie { Id = 9, Title = "Zootopia 2", Director = "Byron Howard & Rich Moore", ReleaseYear = 2025, Genre = "Animation / Adventure", Synopsis = "Judy Hopps and Nick Wilde uncover a new threat to the peace between predators and prey.", DurationMinutes = 105 },
                new Movie { Id = 10, Title = "The Smurfs", Director = "Kelly Asbury", ReleaseYear = 2023, Genre = "Animation / Family", Synopsis = "The Smurfs return to our world to stop Gargamel’s latest evil scheme.", DurationMinutes = 90 },
                new Movie { Id = 11, Title = "Lilo & Stitch (2025)", Director = "Dean Devlin", ReleaseYear = 2025, Genre = "Animation / Adventure", Synopsis = "Lilo and her alien friend Stitch embark on new adventures in the Hawaiian islands.", DurationMinutes = 85 }
            );

            // Връзки
            modelBuilder.Entity<City>()
                .HasMany(c => c.Cinemas)
                .WithOne(cn => cn.City)
                .HasForeignKey(cn => cn.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cinema>()
                .HasMany(cn => cn.Screenings)
                .WithOne(s => s.Cinema)
                .HasForeignKey(s => s.CinemaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Screenings)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Screening>()
                .HasMany(s => s.Seats)
                .WithOne(se => se.Screening)
                .HasForeignKey(se => se.ScreeningId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Payment)
                .WithOne(p => p.Reservation)
                .HasForeignKey<Payment>(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation <-> Seat many-to-many
            modelBuilder.Entity<ReservationSeat>()
                .HasKey(rs => new { rs.ReservationId, rs.SeatId });

            modelBuilder.Entity<ReservationSeat>()
                .HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservationSeats)
                .HasForeignKey(rs => rs.ReservationId);

            modelBuilder.Entity<ReservationSeat>()
                .HasOne(rs => rs.Seat)
                .WithMany(s => s.ReservationSeats)
                .HasForeignKey(rs => rs.SeatId);


            // Индекси
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.ScreeningId, s.Row, s.Number })
                .IsUnique();

            // Decimal прецизност
            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Seat>()
              .HasIndex(s => new { s.ScreeningId, s.Row, s.Number })
              .IsUnique();

            modelBuilder.Entity<ReservationSeat>()
               .HasOne(rs => rs.Reservation)
               .WithMany(r => r.ReservationSeats)
               .HasForeignKey(rs => rs.ReservationId)
               .OnDelete(DeleteBehavior.Restrict); // avoid cascade path

            modelBuilder.Entity<ReservationSeat>()
                .HasOne(rs => rs.Seat)
                .WithMany(s => s.ReservationSeats)
                .HasForeignKey(rs => rs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
