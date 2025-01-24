## Installation: (Requires .Net core 6.0)
Run `dotnet restore` and `dotnet build` to install and download dependencies
## Running the application: 
1. Navigate to DicomMicroservice project and run `dotnet restore` , `dotnet build`
2. Start the application by running `dotnet run` from the root directory:
 /DicomMicroservice
3. Open link http://localhost:5113/swagger/index.html to access the swagger API documentation
## Testing
1. The endpoints can be tested directly from the swagger documentation or from the postman collections available inside the /PostmanCollection folder
2. If testing APIs from postman, Dicom files can be uploaded from the local machine in the request body by choosing the form-data and choosing "File" from the form type to upload a Form file. 
### Unit tests
1. Navigate to the DicomMicroservice.Tests project
2. Run `dotnet build` and `dotnet test`
### Notes
- SampleDcmFiles - contains sample Dicom files used by /extractAttribute endpoint.
- pathToUpload - Directory used by /upload endpoint to save the processed Dicom file
- ConvertedPngFiles - Directory used by /convertToPng endpoint to store the converted PNG files. 
- The same directories are used in the Nunit tests.


