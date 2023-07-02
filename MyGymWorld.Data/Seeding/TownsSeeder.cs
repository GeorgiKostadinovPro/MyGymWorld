namespace MyGymWorld.Data.Seeding
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Seeding.Contracts;
    using System;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class TownsSeeder : ISeeder
    {
        public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Towns.Any())
            {
                return;
            }

            Guid bulgariaId = dbContext.Countries.FirstOrDefault(c => c.Name == "Bulgaria")!.Id;
            Guid unitedStatesId = dbContext.Countries.FirstOrDefault(c => c.Name == "United States")!.Id;
            Guid unitedKingdomId = dbContext.Countries.FirstOrDefault(c => c.Name == "United Kingdom")!.Id;
            Guid russiaId = dbContext.Countries.FirstOrDefault(c => c.Name == "Russia")!.Id;
            Guid southKoreaId = dbContext.Countries.FirstOrDefault(c => c.Name == "South Korea")!.Id;
            Guid chinaId = dbContext.Countries.FirstOrDefault(c => c.Name == "China")!.Id;
            Guid franceId = dbContext.Countries.FirstOrDefault(c => c.Name == "France")!.Id;
            Guid spainId = dbContext.Countries.FirstOrDefault(c => c.Name == "Spain")!.Id;
            Guid germanyId = dbContext.Countries.FirstOrDefault(c => c.Name == "Germany")!.Id;
            Guid italyId = dbContext.Countries.FirstOrDefault(c => c.Name == "Italy")!.Id;

            IEnumerable<Town> towns = new HashSet<Town>()
            {
                new Town
                {
                    Name = "Sofia",
                    Population = 1_236_000,
                    ZipCode = "1000",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = bulgariaId
                },
                new Town
                {
                    Name = "Plovdiv",
                    Population = 343_424,
                    ZipCode = "4000",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = bulgariaId
                },
                new Town
                {
                    Name = "New York City",
                    Population = 8_468_000,
                    ZipCode = "10001",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = unitedStatesId
                },
                new Town
                {
                    Name = "Los Angeles",
                    Population = 3_894_000,
                    ZipCode = "90001",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = unitedStatesId
                },
                new Town
                {
                    Name = "London",
                    Population = 8_982_000,
                    ZipCode = "E17DS",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = unitedKingdomId
                },
                new Town
                {
                    Name = "Manchester",
                    Population = 553_230,
                    ZipCode = "03101",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = unitedKingdomId
                },
                new Town
                {
                    Name = "Moscow",
                    Population = 11_980_000,
                    ZipCode = "101000",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = russiaId
                },
                new Town
                {
                    Name = "Saint Petersburg",
                    Population = 5_384_342,
                    ZipCode = "03101",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = russiaId
                },
                new Town
                {
                    Name = "Seoul",
                    Population = 9_776_000,
                    ZipCode = "01000",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = southKoreaId
                },
                new Town
                {
                    Name = "Busan",
                    Population = 3_678_555,
                    ZipCode = "44972",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = southKoreaId
                },
                new Town
                {
                    Name = "Shanghai",
                    Population = 26_320_000,
                    ZipCode = "200001",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = chinaId
                },
                new Town
                {
                    Name = "Beijing",
                    Population = 21_540_000,
                    ZipCode = "065001",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = chinaId
                },
                new Town
                {
                    Name = "Paris",
                    Population = 2_161_000,
                    ZipCode = "70123",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = franceId
                },
                new Town
                {
                    Name = "Marseille",
                    Population = 861_635,
                    ZipCode = "13000",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = franceId
                },
                new Town
                {
                    Name = "Madrid",
                    Population = 6_642_000,
                    ZipCode = "28001",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = spainId
                },
                new Town
                {
                    Name = "Barcelona",
                    Population = 5_575_000,
                    ZipCode = "08001",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = spainId
                },
                new Town
                {
                    Name = "Berlin",
                    Population = 3_645_000,
                    ZipCode = "10115",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = germanyId
                },
                new Town
                {
                    Name = "Hamburg",
                    Population = 1_841_000,
                    ZipCode = "20095",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = germanyId
                },
                new Town
                {
                    Name = "Rome",
                    Population = 4_316_000,
                    ZipCode = "00042",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = italyId
                },
                new Town
                {
                    Name = "Milan",
                    Population = 1_352_000,
                    ZipCode = "20019",
                    CreatedOn = DateTime.UtcNow,
                    CountryId = italyId
                }
            };

            await dbContext.Towns.AddRangeAsync(towns);
            await dbContext.Towns.AddRangeAsync();
        }
    }
}
