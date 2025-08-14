using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;

namespace CineMate.Data.Seed
{
    public static class CineMateSeeder
    {
        /// <summary>
        /// Слага градове и кина (ако ги няма).
        /// </summary>
        public static async Task SeedCitiesAndCinemasAsync(CineMateDbContext db)
        {

            

            // Град -> списък с кина (по молове)
            var plan = new Dictionary<string, string[]>
            {
                ["Sofia"] = new[]
                {
                    "CineMate Mall of Sofia",
                    "CineMate Paradise Center",
                    "CineMate Serdika Center",
                    "CineMate The Mall",
                    "CineMate Bulgaria Mall"
                },
                ["Plovdiv"] = new[]
                {
                    "CineMate Plovdiv Mall",
                    "CineMate Markovo Tepe Mall"
                },
                ["Varna"] = new[]
                {
                    "CineMate Grand Mall",
                    "CineMate Varna Mall"
                }
            };

            foreach (var kvp in plan)
            {
                var cityName = kvp.Key;
                var cinemas = kvp.Value;

                // Град
                var city = db.Cities.FirstOrDefault(c => c.Name == cityName);
                if (city == null)
                {
                    city = new City { Name = cityName };
                    db.Cities.Add(city);
                    await db.SaveChangesAsync();
                }

                // Кина за града
                foreach (var cinemaName in cinemas)
                {
                    var exists = db.Cinemas.Any(x => x.Name == cinemaName && x.CityId == city.Id);
                    if (!exists)
                    {
                        db.Cinemas.Add(new Cinema
                        {
                            Name = cinemaName,
                            CityId = city.Id
                        });
                    }
                }
            }

            await db.SaveChangesAsync();
        }


    }
}
