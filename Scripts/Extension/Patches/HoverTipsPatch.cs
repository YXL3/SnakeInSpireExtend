using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class HoverTipsPatch : IPatchMethod
{
    public static string PatchId => "hover_tips_patch";

    public static string Description => "HoverTips for Hysteresis and Haste.";

    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(CardModel), "HoverTips", MethodType.Getter)];
    }

    public static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        CheckAddHovertip(__instance, ref __result, "Hysteresis");
        CheckAddHovertip(__instance, ref __result, "Haste");
    }

    private static void CheckAddHovertip(CardModel __instance, ref IEnumerable<IHoverTip> __result, string name)
    {
        if (!Helper.HasCustomDynamic(__instance, name))
        {
            return;
        }
        List<IHoverTip> list = __result.ToList();
        if(name == "Haste")
        {
            list.Add(Helper.SmartHoverTipFromPowers(__instance, name,
            new DynamicVar("HasteDrawingAmount", Helper.ReadHasteDrawingAmount(__instance)),
            new DynamicVar("HastePassingAmount", Helper.HastePassingAmount(__instance))));
        }
        else
        {
            list.Add(Helper.SmartHoverTipFromPowers(__instance, name));
        }
        __result = list;
    }
}