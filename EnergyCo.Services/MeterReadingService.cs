namespace EnergyCo.Services
{
    using EnergyCo.Models;
    using CsvHelper;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Formats.Asn1;
    using EnergyCo.Data.Interfaces;

    public class MeterReadingService : IMeterReadingService
    {
        private readonly IMeterReadingRepository _repository;

        public MeterReadingService(IMeterReadingRepository repository)
        {
            _repository = repository;
        }

        public async Task<MeterReadingResult> ProcessCsvAsync(IFormFile file)
        {
            var result = new MeterReadingResult();

            if (file == null || file.Length == 0)
                return result;

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<MeterReadingCsv>().ToList();

            foreach (var r in records)
            {
                if (!await IsValidAsync(r))
                {
                    result.FailedReadings++;
                    continue;
                }

                if (await _repository.IsDuplicateAsync(r))
                {
                    result.FailedReadings++;
                    continue;
                }

                var latest = await _repository.GetLatestForAccountAsync(r.AccountId);
                var readTime = DateTime.Parse(r.MeterReadingDateTime);

                if (latest != null && latest.ReadingDateTime > readTime)
                {
                    result.FailedReadings++;
                    continue;
                }

                var newReading = new MeterReading
                {
                    AccountId = r.AccountId,
                    MeterReadValue = r.MeterReadValue,
                    ReadingDateTime = readTime
                };

                await _repository.AddAsync(newReading);
                result.SuccessfulReadings++;
            }

            await _repository.SaveChangesAsync();
            return result;
        }

        private async Task<bool> IsValidAsync(MeterReadingCsv r)
        {
            return await _repository.CheckAccountExistsAsync(r.AccountId) &&
                   DateTime.TryParse(r.MeterReadingDateTime, out _) &&
                   Regex.IsMatch(r.MeterReadValue ?? "", @"^\d{5}$");
        }
    }

}
