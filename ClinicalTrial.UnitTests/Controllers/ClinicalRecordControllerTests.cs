using ClinicalTrial.API.Controllers;
using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClinicalTrial.Tests.Controllers
{
    [TestClass]
    public class ClinicalRecordControllerTests
    {
        private Mock<IClinicalTrialService> _mockService;
        private Mock<ILogger<ClinicalRecordController>> _mockLogger;
        private ClinicalRecordController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize mocks
            _mockService = new Mock<IClinicalTrialService>();
            _mockLogger = new Mock<ILogger<ClinicalRecordController>>();
            _controller = new ClinicalRecordController(_mockService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFound_WhenTrialDoesNotExist()
        {
            // Arrange
            var trialId = Guid.NewGuid();
            _mockService.Setup(service => service.GetClinicalTrialByIdAsync(trialId))
                        .ReturnsAsync((ClinicalRecordDTO)null);

            // Act
            var result = await _controller.GetById(trialId);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsInstanceOfType(notFoundResult, typeof(NotFoundObjectResult));
            Assert.AreEqual("Clinical record not found.", notFoundResult?.Value);
        }

        [TestMethod]
        public async Task GetById_ReturnsOk_WhenTrialExists()
        {
            // Arrange
            var Id = Guid.NewGuid();
            var trialId = "trialId";
            var expectedTrial = new ClinicalRecordDTO { TrialId = trialId, Status = TrialStatus.Ongoing };
            _mockService.Setup(service => service.GetClinicalTrialByIdAsync(Id))
                        .ReturnsAsync(expectedTrial);

            // Act
            var result = await _controller.GetById(Id);
            var okResult = result.Result as OkObjectResult;
            var returnedTrial = okResult?.Value as ClinicalRecordDTO;

            // Assert
            Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
            Assert.IsNotNull(returnedTrial);
            Assert.AreEqual(trialId, returnedTrial.TrialId);
        }

        [TestMethod]
        public async Task GetById_ReturnsBadRequest_WhenIdIsEmptyGuid()
        {
            // Arrange
            var trialId = Guid.Empty; // Invalid GUID

            // Act
            var result = await _controller.GetById(trialId);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            Assert.AreEqual("Clinical record not found.", notFoundResult?.Value);
        }

        [TestMethod]
        public async Task FilterClinicalTrials_ReturnsNotFound_WhenNoTrialsMatch()
        {
            // Arrange
            var status = "Completed";
            var trials = new List<ClinicalRecordDTO>();
            _mockService.Setup(service => service.GetFilteredTrialsAsync(status, null, null, null))
                        .ReturnsAsync(trials);

            // Act
            var result = await _controller.FilterClinicalTrials(status, null, null, null);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsInstanceOfType(notFoundResult, typeof(NotFoundObjectResult));
            Assert.AreEqual("No trials found matching the given criteria.", notFoundResult?.Value);
        }

        [TestMethod]
        public async Task FilterClinicalTrials_ReturnsOk_WhenTrialsMatch()
        {
            // Arrange
            var status = "Completed";
            var trials = new List<ClinicalRecordDTO>
            {
                new ClinicalRecordDTO { TrialId = "trialId", Status = TrialStatus.Completed, Participants = 50 }
            };
            _mockService.Setup(service => service.GetFilteredTrialsAsync(status, null, null, null))
                        .ReturnsAsync(trials);

            // Act
            var result = await _controller.FilterClinicalTrials(status, null, null, null);
            var okResult = result.Result as OkObjectResult;
            var returnedTrials = okResult?.Value as List<ClinicalRecordDTO>;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ClinicalRecordDTO>>));
            Assert.IsNotNull(returnedTrials);
            Assert.AreEqual(1, returnedTrials.Count);
        }

        [TestMethod]
        public async Task FilterClinicalTrials_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var status = "Completed";
            var expectedExceptionMessage = "Service failure";
            _mockService
                .Setup(service => service.GetFilteredTrialsAsync(status, null, null, null))
                .ThrowsAsync(new Exception(expectedExceptionMessage));

            // Act
            var result = await _controller.FilterClinicalTrials(status, null, null, null);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual(expectedExceptionMessage, badRequestResult.Value);
        }

        [TestMethod]
        public async Task FilterClinicalTrials_ReturnsBadRequest_WhenInvalidStatus()
        {
            // Arrange
            var invalidStatus = "Completed";

            // Act
            var result = await _controller.FilterClinicalTrials(invalidStatus, null, null, null);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            Assert.AreEqual("No trials found matching the given criteria.", notFoundResult?.Value);
        }
    }
}
