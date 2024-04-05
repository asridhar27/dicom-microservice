public class DicomHeaderExtractionException : Exception
{
    public DicomHeaderExtractionException()
    {
    }

    public DicomHeaderExtractionException(string message) : base(message)
    {
    }

    public DicomHeaderExtractionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
