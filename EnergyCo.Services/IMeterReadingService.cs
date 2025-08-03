namespace EnergyCo.Services
{
    using Microsoft.AspNetCore.Http;
    using EnergyCo.Models;

    public interface IMeterReadingService
    {
        Task<MeterReadingResult> ProcessCsvAsync(IFormFile file);
    }

}
