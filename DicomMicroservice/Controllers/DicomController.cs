
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DicomMicroservice
{
    /// <summary>
    /// Controller to upload, extract and convert Dicom file
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class DicomController : ControllerBase
    {
        private readonly IDicomService _dicomService;

        public DicomController(IDicomService dicomService)
        {
            _dicomService = dicomService;
        }
        
        [HttpPost("uploadDicom")]
        [SwaggerOperation(Summary = "Uploads a DICOM file.")]
        public async Task<IActionResult> UploadDicomFile([SwaggerParameter("The Dicom File to upload", Required =true)]IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("DICOM file is required.");
            try
            {
                var fileId = await _dicomService.UploadDicomFileAsync(file);
                return Ok(fileId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("extractAttribute")]
        [SwaggerOperation(Summary = "Extracts and returns Dicom Attributes based on a Dicom tag.")]
        public async Task<IActionResult> ExtractDicomHeaderAttribute([FromQuery] string dicomTag, [FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(dicomTag)|| string.IsNullOrEmpty(fileName))
                return BadRequest("Filename and tag parameters are required.");
            try 
            {
                var headerAttribute = await _dicomService.ExtractDicomHeaderAttributeAsync(dicomTag,fileName);
                if(headerAttribute == null) 
                {
                    return NotFound("No DICOM files containing the specified tag found.");
                }
                return Ok(headerAttribute);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("convertToPng")]
        [SwaggerOperation(Summary = "Converts a DICOM file to a PNG.")]
        public async Task<IActionResult> ConvertDicomToPng(IFormFile dicomFile)
        {
            if (dicomFile == null || dicomFile.Length == 0)
                return BadRequest("DICOM file is required.");

            try
            {
                var filename = await _dicomService.ConvertDicomToPngAsync(dicomFile);
                return Ok(filename);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
