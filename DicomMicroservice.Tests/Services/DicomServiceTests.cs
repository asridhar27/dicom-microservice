using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FellowOakDicom;
using FellowOakDicom.IO;
using Moq;
using NUnit.Framework;
using DicomMicroservice;

namespace DicomMicroservice.Tests
{
    public class DicomServiceTests
    {
        private  DicomService _service;
        private readonly string uploadDirectory = "../../../../pathToUpload";
        private readonly string sampleDicomFiles = "../../../../SampleDcmFiles";
        private readonly string uploadPngDirectory = "../../../../ConvertedPngFiles";

        [SetUp]
        public void Setup()
        {   
            _service = new DicomService(uploadDirectory,sampleDicomFiles,uploadPngDirectory);
        }

        [Test]
        public async Task UploadDicomFileAsync_ValidFile_Returns_FileId()
        {

            var sampleFilePath = "../../../../SampleDcmFiles/IM000003";

            byte[] fileBytes = await File.ReadAllBytesAsync(sampleFilePath);

            // Mock IFormFile representing the uploaded DICOM file
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("IM000001.dcm");
            formFileMock.Setup(f => f.Length).Returns(fileBytes.Length);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileBytes));
            var fileId = await _service.UploadDicomFileAsync(formFileMock.Object);

            var guidString = "00000000-0000-0000-0000-000000000000";
            var isValidGuid = Guid.TryParse(fileId, out _);

            Assert.IsNotNull(fileId);
            Assert.IsTrue(isValidGuid);
        }

        [Test]
        public async Task ExtractDicomHeaderAttributeAsync_ValidTag_ReturnsAttributeValue()
        {
            string dicomTag = "00100010";
            string fileName = "IM000001.dcm";
            var expectedAttributeValue = new List<string>() {"NAYYAR^HARSH"};
            var headerAttributes = new List<string>();
            var dicomFileMock = new Mock<DicomFile>();
            headerAttributes = await _service.ExtractDicomHeaderAttributeAsync(dicomTag,fileName);

            Assert.IsNotNull(headerAttributes);
            Assert.AreEqual(expectedAttributeValue, headerAttributes);
        }

        [Test]
        public async Task ConvertDicomToPngAsync_ValidFile_ReturnsFileName()
        {
            var dicomFilePath = "../../../../SampleDcmFiles/IM000003"; 
            var dicomFileStream = File.OpenRead(dicomFilePath); //valid DICOM file
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(f => f.OpenReadStream()).Returns(dicomFileStream);

            var result = await _service.ConvertDicomToPngAsync(mockFormFile.Object);

            Assert.That(result, Is.Not.Null.Or.Empty);
            Assert.That(result, Does.EndWith(".png")); 
        }

        [Test]
        public async Task ConvertDicomToPngAsync_InvalidDicomFile_ThrowsException()
        {
            var invalidDicomFilePath = "../../../../InvalidDicomFiles/invalidDicomImage.jpeg";
            var invalidDicomFileStream = File.OpenRead(invalidDicomFilePath); //invalid DICOM file
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(f => f.OpenReadStream()).Returns(invalidDicomFileStream);

            Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                await _service.ConvertDicomToPngAsync(mockFormFile.Object);
            });
        }

        [Test]
        public async Task ConvertDicomToPngAsync_NullDicomFile_ThrowsException()
        {
            IFormFile nullDicomFile = null;
            Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                await _service.ConvertDicomToPngAsync(nullDicomFile);
            });
        }
    }
}

