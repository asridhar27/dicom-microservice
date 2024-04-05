using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using DicomMicroservice;

namespace DicomMicroservice.Tests
{
    [TestFixture]
    public class DicomControllerTests
    {
        private DicomController _controller;
        private Mock<IDicomService> _mockDicomService;

        [SetUp]
        public void Setup()
        {
            _mockDicomService = new Mock<IDicomService>();
            _controller = new DicomController(_mockDicomService.Object);
        }

        [Test]
        public async Task UploadDicomFile_Returns_OkResult_With_FileId()
        {

            _mockDicomService.Setup(s => s.UploadDicomFileAsync(It.IsAny<IFormFile>())).ReturnsAsync("fileId");
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("IM000001.dcm");
            formFileMock.Setup(f => f.Length).Returns(8);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            var fileId = "12345";

            var result = await _controller.UploadDicomFile(formFileMock.Object);

            Assert.IsNotNull(result);
            var objResult = result as ObjectResult;
            Assert.IsNotNull(objResult);
            Assert.AreEqual(200, objResult.StatusCode);
        }

        [Test]
        public async Task ExtractDicomHeaderAttribute_ValidTag_Returns_HeaderAttribute()
        {
            var expectedAttributeValue = new List<string>() {"HeaderAttribute"};
            _mockDicomService.Setup(x => x.ExtractDicomHeaderAttributeAsync(It.IsAny<string>(),It.IsAny<string>()))
                            .ReturnsAsync(expectedAttributeValue);
            var actionResult = await _controller.ExtractDicomHeaderAttribute("00100010", "filename");

            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedAttributeValue, okResult.Value);
        }

        [Test]
        public async Task ConvertDicomToPng_ValidFile_ReturnsOkResult()
        {
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("IM000001.dcm");
            formFileMock.Setup(f => f.Length).Returns(8);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            _mockDicomService.Setup(s => s.ConvertDicomToPngAsync(It.IsAny<IFormFile>())).ReturnsAsync("output_filename.png");

            var result = await _controller.ConvertDicomToPng(formFileMock.Object) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("output_filename.png", result.Value);
        }
    }
}


