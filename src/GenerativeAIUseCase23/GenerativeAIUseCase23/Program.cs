using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Bogus;

namespace SyntheticDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfTitles = 100;
            int numberOfCredits = 300;

            var titles = GenerateTitles(numberOfTitles);
            var credits = GenerateCredits(numberOfCredits, titles);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int srcIndex = baseDirectory.IndexOf("src", StringComparison.OrdinalIgnoreCase);

            if (srcIndex != -1)
            {
                baseDirectory = baseDirectory.Substring(0, srcIndex); //
            }

            // Create a "Results" directory under the project directory
            string resultsDirectory = Path.Combine(baseDirectory, "Results");
            Directory.CreateDirectory(resultsDirectory);

            string titlesFilePath = Path.Combine(resultsDirectory, "titles.csv");
            string creditsFilePath = Path.Combine(resultsDirectory, "credits.csv");

            SaveToCsv(titles, titlesFilePath);
            SaveToCsv(credits, creditsFilePath);

            Console.WriteLine("Datasets generated and saved to CSV files.");
        }

        static List<Title> GenerateTitles(int numberOfTitles)
        {
            var genres = new[] { "Action", "Adventure", "Comedy", "Drama", "Fantasy", "Horror", "Sci-Fi" };
            var certifications = new[] { "G", "PG", "PG-13", "R", "NC-17", "U", "U/A", "A", "S", "AL", "6", "9", "12", "12A", "15", "18", "18R", "R18", "R21", "M", "MA15+", "R16", "R18+", "X18", "T", "E", "E10+", "EC", "C", "CA", "GP", "M/PG", "TV-Y", "TV-Y7", "TV-G", "TV-PG", "TV-14", "TV-MA" };
            var countryCodes = new[] { "USA", "GBR", "CAN", "DEU", "FRA", "JPN", "IND", "UKR" };

            var faker = new Faker<Title>("en_US")
                .RuleFor(t => t.Id, f => f.IndexGlobal)
                .RuleFor(t => t.Name, f => f.Random.Words(f.Random.Int(1, 3)))
                .RuleFor(t => t.Description, f => f.Random.Words(f.Random.Int(10, 20)))
                .RuleFor(t => t.ReleaseYear, f => f.Date.Past(20, DateTime.Now).Year)
                .RuleFor(t => t.AgeCertification, f => f.PickRandom(certifications))
                .RuleFor(t => t.Runtime, f => f.Random.Int(60, 180))
                .RuleFor(t => t.Genres, f => f.PickRandom(genres, f.Random.Int(1, 3)).ToList())
                .RuleFor(t => t.ProductionCountry, f => f.PickRandom(countryCodes))
                .RuleFor(t => t.Seasons, f => f.PickRandom(new string[] { null, f.Random.Int(1, 10).ToString() }))
                .Generate(numberOfTitles);

            return faker;
        }

        static List<Credit> GenerateCredits(int numberOfCredits, List<Title> titles)
        {
            var roles = new[] { "Director", "Producer", "Screenwriter", "Actor", "Actress", "Cinematographer", "Film Editor", "Production Designer", "Costume Designer", "Music Composer" };

            var faker = new Faker<Credit>()
                .RuleFor(c => c.Id, f => f.IndexGlobal)
                .RuleFor(c => c.TitleId, f => f.PickRandom(titles).Id)
                .RuleFor(c => c.RealName, f => f.Name.FullName())
                .RuleFor(c => c.CharacterName, f => f.Name.FirstName())
                .RuleFor(c => c.Role, f => f.PickRandom(roles))
                .Generate(numberOfCredits);

            return faker;
        }

        static void SaveToCsv<T>(List<T> data, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                var csvWriter = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);
                csvWriter.WriteRecords(data);
            }
        }
    }

    public class Title
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public string AgeCertification { get; set; }
        public int Runtime { get; set; }
        public List<string> Genres { get; set; }
        public string ProductionCountry { get; set; }
        public string Seasons { get; set; }
    }

    public class Credit
    {
        public int Id { get; set; }
        public int TitleId { get; set; }
        public string RealName { get; set; }
        public string CharacterName { get; set; }
        public string Role { get; set; }
    }
}