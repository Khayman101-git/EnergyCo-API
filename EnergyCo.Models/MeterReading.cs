namespace EnergyCo.Models
{
    public class MeterReading
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime ReadingDateTime { get; set; }
        public string MeterReadValue { get; set; } = string.Empty;
    }
}
