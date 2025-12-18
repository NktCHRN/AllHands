using AllHands.Shared.Application.Abstractions;

namespace AllHands.Shared.Application.Validation;

public sealed class ImageValidator : IImageValidator
{
    public bool IsValidImage(Stream stream)
    {
        var header = new byte[12];
        var bytesRead = stream.Read(header.AsSpan(0, header.Length));
        
        stream.Position = 0; 

        // JPEG: FF D8 FF
        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return true;

        // PNG: 89 50 4E 47
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
            return true;

        // GIF: "GIF8"
        if (header[0] == 'G' && header[1] == 'I' && header[2] == 'F' && header[3] == '8')
            return true;

        // WebP: "RIFF" .... "WEBP"
        if (bytesRead >= 12 &&
            header[0] == 'R' && header[1] == 'I' && header[2] == 'F' && header[3] == 'F' &&
            header[8] == 'W' && header[9] == 'E' && header[10] == 'B' && header[11] == 'P')
            return true;

        return false;
    }
}
