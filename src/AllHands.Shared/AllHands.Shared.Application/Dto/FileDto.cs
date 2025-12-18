namespace AllHands.Shared.Application.Dto;

public sealed class FileDto
{
    public Stream Stream { get; }
    public string Name { get; }
    public string ContentType { get; }
    public string? OriginalFileName { get; set; }
    
    public FileDto(Stream stream, string name, string contentType)
    {
        Stream = stream;
        Name = name;
        ContentType = contentType;
    }
}
