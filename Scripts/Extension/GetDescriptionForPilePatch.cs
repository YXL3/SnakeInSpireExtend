using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Patching.Models;
using MegaCrit.Sts2.Core.Logging;
using System.Reflection;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class GetDescriptionForPilePatch : IPatchMethod
{
    public static string PatchId => "get_description_for_pile";

    public static string Description => "GetDescriptionForPile for Hysteresis and Haste.";

    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(CardModel), "GetDescriptionForPile", [typeof(PileType), 
        typeof(CardModel).GetNestedType("DescriptionPreviewType", BindingFlags.NonPublic), typeof(Creature)])];
    }
    
    public static void Postfix(CardModel __instance, ref string __result)
    {
        AddStringFromPower(__instance, ref __result, "Hysteresis");
        AddStringFromPower(__instance, ref __result, "Haste");
    }

    private static void AddStringFromPower(CardModel __instance, ref string __result, string name)
    {
        if (!Helper.HasCustomDynamic(__instance, name))
        {
            return;
        }
        string loc = new LocString("powers", $"{name.ToUpper()}.title").GetRawText();
        __result += $"\n[aqua]{loc}{__instance.DynamicVars[name].BaseValue}。[/aqua]";
    }
}