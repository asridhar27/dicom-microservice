using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using FellowOakDicom.IO;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using System.Drawing.Imaging;

public interface IDicomService
{
    Task<string> UploadDicomFileAsync(IFormFile file);
    Task<List<string>> ExtractDicomHeaderAttributeAsync(string dicomTag, string fileName);
    Task<string> ConvertDicomToPngAsync(IFormFile file);
}
