using System.Collections.Frozen;
using AllHands.Application.Abstractions;

namespace AllHands.Application.Utilities;

public sealed class TimeOffEmojiValidator : ITimeOffEmojiValidator
{
    private readonly List<string> _allowedEmoji =
    [
        "✈️",
        "☀️",
        "🏄",
        "🎉",
        "🌴",
        "🤒",
        "😷",
        "🤕",
        "🤧",
        "🏥"
    ];

    private readonly FrozenSet<string> _allowedEmojiSet;

    public TimeOffEmojiValidator()
    {
        _allowedEmojiSet = _allowedEmoji.ToFrozenSet();
    }
    
    public IReadOnlyList<string> AllowedEmoji => _allowedEmoji.ToList();

    public bool IsAllowed(string input)
    {
        return _allowedEmojiSet.Contains(input);
    }
}
