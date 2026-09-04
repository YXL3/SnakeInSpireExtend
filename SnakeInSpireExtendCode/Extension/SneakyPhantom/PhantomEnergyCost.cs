using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace SnakeInSpireExtend.Scripts.Extension;

/// <summary>
/// Serializable snapshot of a card's <see cref="CardEnergyCost"/>. <see cref="CardEnergyCost"/>
/// itself is not serializable (and its members are not exposed publicly), so we capture the
/// fields we care about into this DTO and re-apply them via <see cref="PhantomEnergyCostCodec"/>.
/// </summary>
public sealed class PhantomEnergyCost
{
    /// <summary>
    /// The card's current base energy cost (<c>CardEnergyCost._base</c>), i.e. its cost after
    /// upgrade and any <c>SetCustomBaseCost</c> effects, but before local/global modifiers.
    /// </summary>
    public int BaseCost { get; set; }

    /// <summary>
    /// For X-cost cards, the amount of energy most recently captured/spent. Always 0 for
    /// non-X cards.
    /// </summary>
    public int CapturedXValue { get; set; }

    /// <summary>
    /// The card's local cost modifiers (SetThisCombat / AddUntilPlayed / etc.), applied in order.
    /// </summary>
    public List<PhantomCostModifier> Modifiers { get; set; } = [];
}

/// <summary>
/// Serializable copy of a <see cref="LocalCostModifier"/>.
/// </summary>
public sealed class PhantomCostModifier
{
    public int Amount { get; set; }
    public LocalCostType Type { get; set; }
    public LocalCostModifierExpiration Expiration { get; set; }
    public bool IsReduceOnly { get; set; }
}

/// <summary>
/// Reads a card's energy cost into a <see cref="PhantomEnergyCost"/> and applies it back.
/// Uses reflection to reach the private <c>_localModifiers</c> field, mirroring Bane.cs.
/// Unlike Bane.cs, this also handles X-cost cards (via <see cref="PhantomEnergyCost.CapturedXValue"/>).
/// </summary>
public static class PhantomEnergyCostCodec
{
    private static readonly FieldInfo? LocalModifiersField = typeof(CardEnergyCost)
        .GetField("_localModifiers", BindingFlags.NonPublic | BindingFlags.Instance);

    public static PhantomEnergyCost Capture(CardModel card)
    {
        CardEnergyCost cost = card.EnergyCost;
        PhantomEnergyCost result = new()
        {
            BaseCost = cost.GetWithModifiers(CostModifiers.None),
            CapturedXValue = cost.CostsX ? cost.CapturedXValue : 0,
        };

        if (LocalModifiersField?.GetValue(cost) is List<LocalCostModifier> modifiers)
        {
            foreach (LocalCostModifier modifier in modifiers)
            {
                result.Modifiers.Add(new PhantomCostModifier
                {
                    Amount = modifier.Amount,
                    Type = modifier.Type,
                    Expiration = modifier.Expiration,
                    IsReduceOnly = modifier.IsReduceOnly,
                });
            }
        }

        return result;
    }

    public static void Apply(CardModel card, PhantomEnergyCost? cost)
    {
        if (cost == null) return;
        CardEnergyCost energyCost = card.EnergyCost;

        if (!energyCost.CostsX)
        {
            energyCost.SetCustomBaseCost(cost.BaseCost);
        }
        else if (cost.CapturedXValue != 0)
        {
            energyCost.CapturedXValue = cost.CapturedXValue;
        }

        if (cost.Modifiers is { Count: > 0 } && LocalModifiersField != null)
        {
            List<LocalCostModifier> list = cost.Modifiers
                .Select(m => new LocalCostModifier(m.Amount, m.Type, m.Expiration, m.IsReduceOnly))
                .ToList();
            LocalModifiersField.SetValue(energyCost, list);
            card.InvokeEnergyCostChanged();
        }
    }
}