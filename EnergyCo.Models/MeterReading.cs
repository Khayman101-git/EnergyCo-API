using Microsoft.EntityFrameworkCore;

namespace EnergyCo.Models
{
    [Index(nameof(AccountId))]
    [Index(nameof(AccountId), nameof(ReadingDateTime))]
    public class MeterReading
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime ReadingDateTime { get; set; }
        public string MeterReadValue { get; set; } = string.Empty;
    }
}
