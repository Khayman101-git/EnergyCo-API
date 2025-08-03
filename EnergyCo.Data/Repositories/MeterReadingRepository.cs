namespace EnergyCo.Data.Repositories
{
    using EnergyCo.Data.Interfaces;
    using EnergyCo.Models;
    using Microsoft.EntityFrameworkCore;

    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly AppDbContext _context;

        public MeterReadingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDuplicateAsync(MeterReadingCsv csv)
        {
            return await _context.MeterReadings.AsNoTracking().AnyAsync(m =>
                m.AccountId == csv.AccountId &&
                m.MeterReadValue == csv.MeterReadValue &&
                m.ReadingDateTime == DateTime.Parse(csv.MeterReadingDateTime));
        }

        public async Task<MeterReading?> GetLatestForAccountAsync(int accountId)
        {
            return await _context.MeterReadings.AsNoTracking()
                .Where(m => m.AccountId == accountId)
                .OrderByDescending(m => m.ReadingDateTime)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(MeterReading reading)
        {
            await _context.MeterReadings.AddAsync(reading);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckAccountExistsAsync(int accountId)
        {
            return await _context.Accounts.AnyAsync(m => m.AccountId == accountId);
        }
    }

}
