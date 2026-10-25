using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace SnakeInSpireExtend.Scripts.Extension.HoverTips;

/// <summary>
/// A single <see cref="IHoverTip"/> slot that cycles through several card previews.
/// The contained tips are real <see cref="CardHoverTip"/> instances, so normal card
/// preview rendering can be reused.
/// </summary>
public sealed class CyclingCardHoverTip : CardHoverTip
{
    public const float DefaultIntervalSeconds = 1f;

    public IReadOnlyList<CardHoverTip> Tips { get; }

    public float IntervalSeconds { get; }

    public CyclingCardHoverTip(IEnumerable<CardModel> cards, float intervalSeconds = DefaultIntervalSeconds)
        : this(cards.Select(static card => new CardHoverTip(card)).ToArray(), intervalSeconds)
    {
    }

    private CyclingCardHoverTip(IReadOnlyList<CardHoverTip> tips, float intervalSeconds)
        : base(GetFirstCard(tips))
    {
        if (tips.Count == 0)
        {
            throw new ArgumentException("At least one card is required.", nameof(tips));
        }

        if (intervalSeconds <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(intervalSeconds), "The interval must be positive.");
        }

        Tips = tips;
        IntervalSeconds = intervalSeconds;
    }

    private static CardModel GetFirstCard(IReadOnlyList<CardHoverTip> tips)
    {
        if (tips.Count == 0)
        {
            throw new ArgumentException("At least one card is required.", nameof(tips));
        }

        return tips[0].Card;
    }
}
