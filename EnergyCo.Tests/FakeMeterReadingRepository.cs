using EnergyCo.Data.Interfaces;
using EnergyCo.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyCo.Tests
{

    public class FakeMeterReadingRepository : IMeterReadingRepository
    {
        public List<MeterReading> StoredReadings { get; set; } = new();
        public List<Account> StoredAccounts { get; set; } = new();

        public Task AddAsync(MeterReading reading)
        {
            StoredReadings.Add(reading);
            return Task.CompletedTask;
        }

        public async Task<bool> CheckAccountExistsAsync(int accountId)
        {
            return StoredAccounts.Any(m => m.AccountId == accountId);
        }

        public Task<MeterReading?> GetLatestForAccountAsync(int accountId)
        {
            var latest = StoredReadings
                .Where(r => r.AccountId == accountId)
                .OrderByDescending(r => r.ReadingDateTime)
                .FirstOrDefault();

            return Task.FromResult(latest);
        }

        public Task<bool> IsDuplicateAsync(MeterReadingCsv csv)
        {
            return Task.FromResult(StoredReadings.Any(m =>
                m.AccountId == csv.AccountId &&
                m.MeterReadValue == csv.MeterReadValue &&
                m.ReadingDateTime == DateTime.Parse(csv.MeterReadingDateTime)));
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

}
