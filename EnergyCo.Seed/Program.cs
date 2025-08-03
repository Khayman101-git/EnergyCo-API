using CsvHelper;
using System.Globalization;
using EnergyCo.Data;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using EnergyCo.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "EnergyCo.Data", "energyco.db");
        var connectionString = $"Data Source={Path.GetFullPath(dbPath)}";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .Options;

        using var context = new AppDbContext(options);

        // Read CSV

        using (var reader = new StreamReader("test-accounts.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var accounts = csv.GetRecords<Account>().ToList();

            foreach (var acc in accounts)
            {
                if (context.Accounts.Any(a => a.AccountId == acc.AccountId))
                {
                    Console.WriteLine($"Duplicate Account: {acc.AccountId}");
                    continue;
                }

                context.Accounts.Add(acc);
            }
            context.SaveChanges();
        }

        Console.WriteLine("Seeding complete.");

    }
}