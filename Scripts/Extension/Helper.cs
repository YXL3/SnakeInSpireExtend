using System.Reflection;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Cards;
using SnakeInSpireExtend.Scripts.Powers;

namespace SnakeInSpireExtend.Scripts.Extension;
public static class Helper
{
    private static FieldInfo? _varsField;
    
    static Helper()
    {
        _varsField = typeof(DynamicVarSet).GetField("_vars", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public static void Hysteresis(CardModel card, decimal amount)
    {
        AddDynamicToCard(card, "Hysteresis", amount);
    }

    public static void Haste(CardModel card, decimal amount)
    {
        AddDynamicToCard(card, "Haste", amount);
    }
    
    public static void AddDynamicToCard(CardModel card, string name, decimal amount)
    {
        DynamicVarSet vars = card.DynamicVars;
        if (!vars.ContainsKey(name))
        {
            TryAddDynamicVar(vars, new DynamicVar(name, amount));
        }
        else
        {
            vars[name].UpgradeValueBy(amount);
        }
    }
    
    private static bool TryAddDynamicVar(this DynamicVarSet target, DynamicVar newVar)
    {
        if (_varsField == null) return false;
        var vars = _varsField.GetValue(target) as Dictionary<string, DynamicVar>;
        if (vars == null || vars.ContainsKey(newVar.Name)) return false;
        vars.Add(newVar.Name, newVar);
        return true;
    }

    public static HoverTip StaticHoverTipFromPowers(string name, params DynamicVar[] descriptionVars)
    {
        LocString locString = new LocString("powers", name + ".title");
        LocString locString2 = new LocString("powers", name + ".description");
        foreach(DynamicVar item in descriptionVars)
        {
            locString2.Add(item);
        }
        return new HoverTip(locString, locString2);
    }

    public static HoverTip SmartHoverTipFromPowers(CardModel card, string name, params DynamicVar[] descriptionVars)
    {
        LocString locString = new LocString("powers", $"{name.ToUpper()}.smartTitle");
        LocString locString2 = new LocString("powers", $"{name.ToUpper()}.smartDescription");
        locString.Add(card.DynamicVars[name]);
        locString2.Add(card.DynamicVars[name]);
        foreach(DynamicVar item in descriptionVars)
        {
            locString2.Add(item);
        }
        return new HoverTip(locString, locString2);
    }

    private static HoverTip HysteresisHoverTip()
    {
        return StaticHoverTipFromPowers("HYSTERESIS");
    }

    private static HoverTip HasteHoverTip(CardModel card)
    {
        return StaticHoverTipFromPowers("HASTE", new DynamicVar("HasteDrawingAmount", 1m + GetOwnerPowerAmount<PsychokinesisPower>(card)));
    }

    public static IEnumerable<IHoverTip> ConditionalHoverTip(CardModel card, string name, Func<HoverTip> tipFactory)
    {
        if (!HasCustomDynamic(card, name))yield return tipFactory();
    }

    public static IEnumerable<IHoverTip> HysteresisHoverTipIfNeeded(CardModel card) => ConditionalHoverTip(card, "Hysteresis", HysteresisHoverTip);

    public static IEnumerable<IHoverTip> HasteHoverTipIfNeeded(CardModel card) => ConditionalHoverTip(card, "Haste", () => HasteHoverTip(card));

    public static HoverTip HasteHoverTip(RelicModel relic)
    {
        return StaticHoverTipFromPowers("HASTE", new DynamicVar("HasteDrawingAmount", 1m + GetOwnerPowerAmount<PsychokinesisPower>(relic)));
    }

    public static bool HasCustomDynamic(CardModel card, string name)
    {
        return card.DynamicVars.ContainsKey(name) && card.DynamicVars[name].BaseValue > 0;
    }

    public static decimal GetOwnerPowerAmount<T>(CardModel card) where T : PowerModel
    {
        if ((!card.IsMutable) || card.Owner == null || !card.Owner.Creature.HasPower<T>())
        {
            return 0m;
        }
        return card.Owner.Creature.GetPower<T>()!.Amount;
    }

    public static decimal GetOwnerPowerAmount<T>(RelicModel relic) where T : PowerModel
    {
        if ((!relic.IsMutable) || relic.Owner == null || !relic.Owner.Creature.HasPower<T>())
        {
            return 0m;
        }
        return relic.Owner.Creature.GetPower<T>()!.Amount;
    }

    public static decimal ReadHasteDrawingAmount(CardModel card)
    {
        decimal result = 1m + GetOwnerPowerAmount<PsychokinesisPower>(card);
        if (CardTypesOfHasteDrawingAmount.Contains(card.GetType()))
        {
            result += card.DynamicVars["HasteDrawingAmount"].BaseValue;
        }
        if ((!card.IsMutable) || card.Owner == null)
        {
            return result;
        }
        foreach (RelicModel relic in card.Owner.Relics)
        {
            if (relic is IHasteModifier)
            {
                result += ((IHasteModifier)relic).ReadHasteModifier(card, result);
            }
        }
        return result;
    }

    public static async Task<decimal> ApplyHasteDrawingAmount(CardModel card)
    {
        decimal result = 1m + GetOwnerPowerAmount<PsychokinesisPower>(card);
        if (CardTypesOfHasteDrawingAmount.Contains(card.GetType()))
        {
            result += card.DynamicVars["HasteDrawingAmount"].BaseValue;
        }
        if ((!card.IsMutable) || card.Owner == null)
        {
            return result;
        }
        foreach (RelicModel relic in card.Owner.Relics)
        {
            if (relic is IHasteModifier)
            {
                result += await ((IHasteModifier)relic).ApplyHasteModifier(card, result);
            }
        }
        return result;
    }

    public static decimal HastePassingAmount(CardModel card)
    {
        decimal result = card.DynamicVars["Haste"].BaseValue - 1m;
        return result;
    }

    private static readonly HashSet<Type> CardTypesOfHasteDrawingAmount = new()
    {
        typeof(Shelter)
    };
}

public interface IHasteModifier
{
    decimal ReadHasteModifier(CardModel card, decimal currentValue);

    virtual Task<decimal> ApplyHasteModifier(CardModel card, decimal currentValue) => Task.FromResult(ReadHasteModifier(card, currentValue));
}