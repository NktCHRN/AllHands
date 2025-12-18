namespace AllHands.Shared.Application.Abstractions;

public interface IImageValidator
{
    bool IsValidImage(Stream stream);
}
