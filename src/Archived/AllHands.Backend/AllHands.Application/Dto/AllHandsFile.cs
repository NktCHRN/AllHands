namespace AllHands.Application.Dto;

public sealed class AllHandsFile
{
    public Stream Stream { get; }
    public string Name { get; }
    public string ContentType { get; }
    public string? OriginalFileName { get; set; }
    
    public AllHandsFile(Stream stream, string name, string contentType)
    {
        Stream = stream;
        Name = name;
        ContentType = contentType;
    }
}
