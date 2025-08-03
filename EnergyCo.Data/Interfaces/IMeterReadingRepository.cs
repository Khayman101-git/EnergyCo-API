namespace EnergyCo.Data.Interfaces
{
    using EnergyCo.Models;

    public interface IMeterReadingRepository
    {
        Task<bool> IsDuplicateAsync(MeterReadingCsv csv);

        Task<MeterReading?> GetLatestForAccountAsync(int accountId);

        Task<bool> CheckAccountExistsAsync(int accountId);

        Task AddAsync(MeterReading reading);

        Task SaveChangesAsync();
    }

}
