using FellowOakDicom;
using FellowOakDicom.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class DicomService : IDicomService
{
    private readonly string _uploadDirectory;
    private readonly string _uploadPNGFiles;
    private readonly string _dicomDirectory;

    public DicomService(string uploadDirectory, string dicomDirectory, string uploadPngFiles)
    {
        _uploadDirectory = uploadDirectory;
        _dicomDirectory = dicomDirectory;
        _uploadPNGFiles = uploadPngFiles;
    }
    
    public async Task<string> UploadDicomFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("DICOM file is required.");

        try
        {
            var dicomFile = DicomFile.Open(file.OpenReadStream());
            if(dicomFile == null)
            {
                throw new ArgumentException("Invalid DICOM file");
            }

            var fileId = Guid.NewGuid().ToString(); // Generate unique file ID
            var filePath = Path.Combine(_uploadDirectory, fileId + ".dcm");
            dicomFile.Save(filePath);
            return fileId;
        } 
        catch (Exception ex)
        {
            throw new Exception("Error processing DICOM file", ex);
        }

    }

    public async Task<List<string>> ExtractDicomHeaderAttributeAsync(string dicomTag, string fileName)
    { 
        var filePath = Path.Combine(_dicomDirectory, fileName);
        var headerAttributes = new List<string>();
        try
        {
            var extractedFile = await DicomFile.OpenAsync(filePath).ConfigureAwait(false);
            if(extractedFile != null && extractedFile.Dataset.Contains(DicomTag.Parse(dicomTag)))
            {
                headerAttributes.Add(extractedFile.Dataset.GetSingleValue<string>(DicomTag.Parse(dicomTag)));
            }

            return headerAttributes;
            
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new DicomHeaderExtractionException ("Directory not found", ex);
        }
        catch (DicomDataException)
        {
            throw new DicomHeaderExtractionException ("Invalid Filename or Dicom Tag");
        }

    }

    public async Task<string> ConvertDicomToPngAsync(IFormFile dicomFile)
    {
        try
        {

            DicomFile file = DicomFile.Open(dicomFile.OpenReadStream());
            var outputFileName = Guid.NewGuid().ToString() + ".png";
            var outputPath = Path.Combine(_uploadPNGFiles, outputFileName);

            var imageRendered = new DicomImage(file.Dataset).RenderImage();
            var imageSharpImg = new Image<Rgba32>(imageRendered.Width, imageRendered.Height);
            for (int y=0; y<imageRendered.Height; y++)
            {
                for (int x=0; x<imageRendered.Width; x++)
                {
                    var pixelData = imageRendered.GetPixel(x,y);
                    imageSharpImg[x,y] = new Rgba32(pixelData.R, pixelData.G,pixelData.B, pixelData.A);

                }
            }

            using (var fs = new FileStream(outputPath, FileMode.Create))
            {
                imageSharpImg.SaveAsPng(fs);
            }

            return outputFileName;

        }
        catch (DicomFileException ex)
        {
            throw new ApplicationException("Failed to open DICOM file", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to convert DICOM to PNG", ex);
        }
    }

}
