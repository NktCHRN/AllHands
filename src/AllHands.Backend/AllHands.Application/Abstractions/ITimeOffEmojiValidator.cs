namespace AllHands.Application.Abstractions;

public interface ITimeOffEmojiValidator
{
    IReadOnlyList<string> AllowedEmoji { get; }
    bool IsAllowed(string input);
}