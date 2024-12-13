using ClinicalTrial.Business.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinicalTrial.Business.Interfaces;

namespace ClinicalTrial.Tests.Business
{
    [TestClass]
    public class ClinicalTrialServiceTests
    {
        private Mock<IClinicalTrialRepository> _mockRepository;
        private Mock<ILogger<ClinicalTrialService>> _mockLogger;
        private ClinicalTrialService _clinicalTrialService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IClinicalTrialRepository>();
            _mockLogger = new Mock<ILogger<ClinicalTrialService>>();
            _clinicalTrialService = new ClinicalTrialService(_mockRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task ProcessFileAsync_ReturnsSuccess_WhenJsonIsValid()
        {
            // Arrange
            var generatedGuid = Guid.NewGuid();
            var mockFile = CreateMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"endDate\": \"2024-01-31\", \"participants\": 10, \"status\": \"Ongoing\"}");
            _mockRepository.Setup(repo => repo.AddClinicalTrialAsync(It.IsAny<ClinicalTrial.Business.Models.ClinicalTrial>()))
                           .ReturnsAsync(generatedGuid);

            // Act
            var result = await _clinicalTrialService.ProcessFileAsync(mockFile);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("File processed successfully.", result.Message);
        }

        [TestMethod]
        public async Task ProcessFileAsync_ReturnsFailure_WhenJsonIsInvalid()
        {
            // Arrange
            var mockFile = CreateMockFile("{\"invalid\": \"json\"}");
            _mockRepository.Setup(repo => repo.AddClinicalTrialAsync(It.IsAny<ClinicalTrial.Business.Models.ClinicalTrial>()))
                           .ThrowsAsync(new Exception("Unexpected call"));

            // Act
            var result = await _clinicalTrialService.ProcessFileAsync(mockFile);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid JSON schema.", result.Message);
        }

        [TestMethod]
        public async Task ProcessFileAsync_AddsEndDate_WhenStatusIsOngoingAndEndDateIsDefault()
        {
            // Arrange
            var generatedGuid = Guid.NewGuid();
            var mockFile = CreateMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"endDate\": \"0001-01-01\", \"participants\": 10, \"status\": \"Ongoing\"}");
            ClinicalTrial.Business.Models.ClinicalTrial addedTrial = null;
            _mockRepository.Setup(repo => repo.AddClinicalTrialAsync(It.IsAny<ClinicalTrial.Business.Models.ClinicalTrial>()))
                           .Callback<ClinicalTrial.Business.Models.ClinicalTrial>(trial => addedTrial = trial)
                           .ReturnsAsync(generatedGuid);

            // Act
            var result = await _clinicalTrialService.ProcessFileAsync(mockFile);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("File processed successfully.", result.Message);
            Assert.IsNotNull(addedTrial);
            Assert.AreEqual(new DateTime(2024, 02, 01), addedTrial.EndDate);
        }

        [TestMethod]
        public async Task ProcessFileAsync_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var mockFile = CreateMockFile("{\"trialId\": \"123\", \"title\": \"Test Trial\", \"startDate\": \"2024-01-01\", \"endDate\": \"2024-01-31\", \"participants\": 10, \"status\": \"Ongoing\"}");
            _mockRepository.Setup(repo => repo.AddClinicalTrialAsync(It.IsAny<ClinicalTrial.Business.Models.ClinicalTrial>()))
                           .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _clinicalTrialService.ProcessFileAsync(mockFile);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Error processing file.", result.Message);
        }

        [TestMethod]
        public async Task ProcessFileAsync_ReturnsFailure_WhenFileStreamThrowsException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(file => file.OpenReadStream())
                    .Throws(new IOException("File read error"));

            // Act
            var result = await _clinicalTrialService.ProcessFileAsync(mockFile.Object);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Error processing file.", result.Message);
        }

        [TestMethod]
        public async Task ProcessFileAsync_ReturnsFailure_WhenJsonIsMalformed()
        {
            // Arrange
            var mockFile = CreateMockFile("This is not valid JSON");
            _mockRepository.Setup(repo => repo.AddClinicalTrialAsync(It.IsAny<ClinicalTrial.Business.Models.ClinicalTrial>()))
                           .ThrowsAsync(new Exception("Unexpected call"));

            // Act
            var result = await _clinicalTrialService.ProcessFileAsync(mockFile);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid JSON schema.", result.Message);
        }

        [TestMethod]
        public async Task GetClinicalTrialByIdAsync_ValidId_ReturnsClinicalRecord()
        {
            // Arrange
            var validId = Guid.NewGuid();
            var clinicalTrial = new ClinicalTrial.Business.Models.ClinicalTrial
            {
                TrialId = "Test trial Id",
                Status = "Completed",
                StartDate = DateTime.UtcNow.AddMonths(-2),
                EndDate = DateTime.UtcNow.AddMonths(-1)
            };

            _mockRepository
                .Setup(repo => repo.GetClinicialTrialByIdAsync(validId))
                .ReturnsAsync(clinicalTrial);

            // Act
            var result = await _clinicalTrialService.GetClinicalTrialByIdAsync(validId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(clinicalTrial.TrialId, result.TrialId);
            Assert.AreEqual(clinicalTrial.Status, result.Status.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetClinicalTrialByIdAsync_Database_Error()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.GetClinicialTrialByIdAsync(invalidId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            await _clinicalTrialService.GetClinicalTrialByIdAsync(invalidId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task GetClinicalTrialByIdAsync_InvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.GetClinicialTrialByIdAsync(invalidId))
                .ReturnsAsync((ClinicalTrial.Business.Models.ClinicalTrial)null);

            // Act
            await _clinicalTrialService.GetClinicalTrialByIdAsync(invalidId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        public async Task GetClinicalTrialByIdAsync_Exception_LogsErrorAndThrows()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.GetClinicialTrialByIdAsync(id))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                async () => await _clinicalTrialService.GetClinicalTrialByIdAsync(id));

            Assert.AreEqual("Unexpected error", exception.Message);
        }

        [TestMethod]
        public async Task GetClinicalTrialByIdAsync_TransformsDTOToRepresentationModel()
        {
            // Arrange
            var id = Guid.NewGuid();
            var clinicalTrial = new ClinicalTrial.Business.Models.ClinicalTrial
            {
                TrialId = "Test trial Id",
                Status = "Ongoing",
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow,
                DurationInDays = 30
            };

            _mockRepository
                .Setup(repo => repo.GetClinicialTrialByIdAsync(id))
                .ReturnsAsync(clinicalTrial);

            // Act
            var result = await _clinicalTrialService.GetClinicalTrialByIdAsync(id);

            // Assert
            Assert.AreEqual(clinicalTrial.TrialId, result.TrialId);
            Assert.AreEqual(clinicalTrial.Status, result.Status.ToString());
            Assert.AreEqual(clinicalTrial.DurationInDays, result.DurationInDays);
        }

        [TestMethod]
        public async Task GetClinicalTrialByIdAsync_NullDTO_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockRepository
                .Setup(repo => repo.GetClinicialTrialByIdAsync(id))
                .ReturnsAsync((ClinicalTrial.Business.Models.ClinicalTrial)null);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                async () => await _clinicalTrialService.GetClinicalTrialByIdAsync(id));

            Assert.AreEqual("Clinical record not found!", exception.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public async Task GetFilteredTrialsAsync_InvalidStatus_ThrowsInvalidDataException()
        {
            // Arrange
            var invalidStatus = "InvalidStatus";

            // Act
            await _clinicalTrialService.GetFilteredTrialsAsync(status: invalidStatus);

            // Assert (Handled by ExpectedException)
            _mockLogger.Verify(logger =>
                logger.LogError("Invalid status provided. Please provide a valid status."), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public async Task GetFilteredTrialsAsync_InvalidParticipants_ThrowsInvalidDataException()
        {
            // Arrange
            var invalidParticipants = -5;

            // Act
            await _clinicalTrialService.GetFilteredTrialsAsync(minParticipants: invalidParticipants);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public async Task GetFilteredTrialsAsync_StartDateGreaterThanEndDate_ThrowsInvalidDataException()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(10);
            var endDate = DateTime.UtcNow;

            // Act
            await _clinicalTrialService.GetFilteredTrialsAsync(startDate: startDate, endDate: endDate);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        public async Task GetFilteredTrialsAsync_ValidFilters_ReturnsClinicalRecords()
        {
            // Arrange
            var clinicalTrials = new List<ClinicalTrial.Business.Models.ClinicalTrial>
        {
            new ClinicalTrial.Business.Models.ClinicalTrial { Id = Guid.NewGuid(), TrialId = "Test Trial Id", Status = "Ongoing", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(-5) },
            new ClinicalTrial.Business.Models.ClinicalTrial { Id = Guid.NewGuid(), TrialId = "Test Trial Id", Status = "Completed", StartDate = DateTime.UtcNow.AddDays(-20), EndDate = DateTime.UtcNow.AddDays(-10) }
        };

            _mockRepository
                .Setup(repo => repo.GetFilteredTrialsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(clinicalTrials);

            // Act
            var result = await _clinicalTrialService.GetFilteredTrialsAsync(status: "Ongoing");

            // Assert
            Assert.AreEqual(2, result.Count());
            _mockRepository.Verify(repo => repo.GetFilteredTrialsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
        }

        [TestMethod]
        public async Task GetFilteredTrialsAsync_NoResults_LogsWarningAndReturnsEmptyList()
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.GetFilteredTrialsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<ClinicalTrial.Business.Models.ClinicalTrial>());

            // Act
            var result = await _clinicalTrialService.GetFilteredTrialsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetFilteredTrialsAsync_TransformsDTOsToRepresentationModels()
        {
            // Arrange
            var clinicalTrials = new List<ClinicalTrial.Business.Models.ClinicalTrial>
        {
            new ClinicalTrial.Business.Models.ClinicalTrial { Id = Guid.NewGuid(), TrialId = "Test Trial Id", Status = "Ongoing", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(-5) }
        };

            _mockRepository
                .Setup(repo => repo.GetFilteredTrialsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(clinicalTrials);

            // Act
            var result = await _clinicalTrialService.GetFilteredTrialsAsync();

            // Assert
            Assert.AreEqual(clinicalTrials.Count, result.Count());
            Assert.AreEqual(clinicalTrials.First().TrialId, result.First().TrialId);
            Assert.AreEqual(clinicalTrials.First().Status, result.First().Status.ToString());
        }

        private IFormFile CreateMockFile(string content)
        {
            var mockFile = new Mock<IFormFile>();
            var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(contentBytes);

            mockFile.Setup(file => file.OpenReadStream()).Returns(stream);
            mockFile.Setup(file => file.Length).Returns(contentBytes.Length);
            mockFile.Setup(file => file.FileName).Returns("mockfile.json");

            return mockFile.Object;
        }
    }
}
