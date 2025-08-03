using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnergyCo.Tester
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using global::EnergyCo.Services;
    using global::EnergyCo.Models;
    using Microsoft.AspNetCore.Http;

    namespace EnergyCo.Tests
    {
        [TestClass]
        public class MeterReadingServiceTests
        {
            private MeterReadingService _service = null!;
            private FakeMeterReadingRepository _repository = null!;

            [TestInitialize]
            public void Setup()
            {
                _repository = new FakeMeterReadingRepository();
                _service = new MeterReadingService(_repository);
            }

            [TestMethod]
            public async Task ValidCsv_ShouldReturnSuccess()
            {
                // Arrange
                // Arrange: preload a duplicate
                _repository.StoredAccounts.Add(new Account
                {
                    AccountId = 123,
                    FirstName = "Bob",
                    LastName = "Smith"
                });
                var csv = new StringBuilder();
                csv.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
                csv.AppendLine("123,2024-01-01T10:00:00,12345");

                var file = CreateFormFile(csv.ToString());

                // Act
                var result = await _service.ProcessCsvAsync(file);

                // Assert
                Assert.AreEqual(1, result.SuccessfulReadings);
                Assert.AreEqual(0, result.FailedReadings);
            }

            [TestMethod]
            public async Task DuplicateCsv_ShouldFail()
            {
                // Arrange: preload a duplicate
                _repository.StoredAccounts.Add(new Account
                {
                    AccountId = 123,
                    FirstName = "Bob",
                    LastName = "Smith"
                });
                _repository.StoredReadings.Add(new MeterReading
                {
                    AccountId = 123,
                    ReadingDateTime = new DateTime(2024, 01, 01, 10, 0, 0),
                    MeterReadValue = "12345"
                });

                var csv = new StringBuilder();
                csv.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
                csv.AppendLine("123,2024-01-01T10:00:00,12345");

                var file = CreateFormFile(csv.ToString());

                // Act
                var result = await _service.ProcessCsvAsync(file);

                // Assert
                Assert.AreEqual(0, result.SuccessfulReadings);
                Assert.AreEqual(1, result.FailedReadings);
            }

            [TestMethod]
            public async Task EntryWithNoAccount_ShouldFail()
            {
                // Arrange: preload a duplicate
                _repository.StoredAccounts.Add(new Account
                {
                    AccountId = 123,
                    FirstName = "Bob",
                    LastName = "Smith"
                });
          
                var csv = new StringBuilder();
                csv.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
                csv.AppendLine("124,2024-01-01T10:00:00,12345");

                var file = CreateFormFile(csv.ToString());

                // Act
                var result = await _service.ProcessCsvAsync(file);

                // Assert
                Assert.AreEqual(0, result.SuccessfulReadings);
                Assert.AreEqual(1, result.FailedReadings);
            }

            [TestMethod]
            public async Task NewReadIsOlder_ShouldFail()
            {
                // Arrange: preload a duplicate
                _repository.StoredAccounts.Add(new Account
                {
                    AccountId = 123,
                    FirstName = "Bob",
                    LastName = "Smith"
                });
                _repository.StoredReadings.Add(new MeterReading
                {
                    AccountId = 123,
                    ReadingDateTime = new DateTime(2025, 01, 02, 10, 0, 0),
                    MeterReadValue = "12345"
                });

                var csv = new StringBuilder();
                csv.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
                csv.AppendLine("123,2025-01-01T10:00:00,12345");

                var file = CreateFormFile(csv.ToString());

                // Act
                var result = await _service.ProcessCsvAsync(file);

                // Assert
                Assert.AreEqual(0, result.SuccessfulReadings);
                Assert.AreEqual(1, result.FailedReadings);
            }

            private IFormFile CreateFormFile(string content)
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                var stream = new MemoryStream(bytes);
                return new FormFile(stream, 0, bytes.Length, "file", "test.csv")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/csv"
                };
            }
        }
    }

}
