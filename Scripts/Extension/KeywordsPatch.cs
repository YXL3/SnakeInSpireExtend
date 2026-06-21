using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Patching.Models;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class KeywordsPatch : IPatchMethod
{
    public static string PatchId => "key_words_patch";

    public static string Description => "Keywords for Keen.";

    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(CardModel), "Keywords", MethodType.Getter)];
    }

    public static void Postfix(CardModel __instance, ref IReadOnlySet<CardKeyword> __result)
    {
        
        HashSet<CardKeyword> newKeywords = new HashSet<CardKeyword>();
        if(hasOuroboros(__instance) && (Helper.HasCustomDynamic(__instance, "Haste") || Helper.HasCustomDynamic(__instance, "Hysteresis")))
        {
            newKeywords.Add(SnakeInSpireExtendCardKeywords.Keen);
        }
        newKeywords.UnionWith(__result);
        __result = newKeywords;
    }

    
    private static bool hasOuroboros(CardModel card)
    {
        return card.IsMutable && card.Owner != null && card.Owner.Creature.HasPower<OuroborosPower>();
    }
}