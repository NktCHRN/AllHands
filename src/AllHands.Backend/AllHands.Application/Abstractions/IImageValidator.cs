namespace AllHands.Application.Abstractions;

public interface IImageValidator
{
    bool IsValidImage(Stream stream);
}
