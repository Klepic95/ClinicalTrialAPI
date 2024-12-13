using ClinicalTrial.API.Controllers;
using ClinicalTrial.Business.Interfaces;
using ClinicalTrial.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalTrial.Tests.Controllers
{
    [TestClass]
    public class ClinicalFileControllerTests
    {
        private Mock<IClinicalTrialService> _mockClinicalTrialService;
        private Mock<ILogger<ClinicalFileController>> _mockLogger;
        private ClinicalFileController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockClinicalTrialService = new Mock<IClinicalTrialService>();
            _mockLogger = new Mock<ILogger<ClinicalFileController>>();
            _controller = new ClinicalFileController(_mockClinicalTrialService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsNull()
        {
            // Arrange
            IFormFile file = null;

            // Act
            var result = await _controller.UploadFile(file);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid file type.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsEmpty()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act
            var result = await _controller.UploadFile(fileMock.Object);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid file type.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsNotJson()
        {
            // Arrange
            var fileMock = CreateMockFile("test.txt", "This is a test file.");

            // Act
            var result = await _controller.UploadFile(fileMock);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Invalid file type.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UploadFile_ReturnsBadRequest_WhenProcessingFails()
        {
            // Arrange
            var fileMock = CreateMockFile("test.json", "{ \"valid\": true }");
            _mockClinicalTrialService
                .Setup(s => s.ProcessFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(new ProcessClinicalFileDTO { IsSuccess = false, Message = "Processing failed." });

            // Act
            var result = await _controller.UploadFile(fileMock);
            var badRequestResult = result.Result as BadRequestObjectResult;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            Assert.AreEqual("Processing failed.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task UploadFile_ReturnsOk_WhenProcessingSucceeds()
        {
            // Arrange
            var generatedGuid = Guid.NewGuid();
            var fileMock = CreateMockFile("test.json", "{ \"valid\": true }");
            _mockClinicalTrialService
                .Setup(s => s.ProcessFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(new ProcessClinicalFileDTO { IsSuccess = true, Id = generatedGuid });

            // Act
            var result = await _controller.UploadFile(fileMock);
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.AreEqual($"File processed successfully. Id of the processed file is: {generatedGuid}", okResult.Value);
        }

        private IFormFile CreateMockFile(string fileName, string content)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.ContentType).Returns("application/json");
            return fileMock.Object;
        }
    }
}
