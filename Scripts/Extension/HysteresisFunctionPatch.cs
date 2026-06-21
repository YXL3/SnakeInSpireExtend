using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class HysteresisFunctionPatch : IPatchMethod
{
    public static string PatchId => "hysteresis_function";

    public static string Description => "Hysteresis function";

    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(CardModel), "GetResultPileTypeForCardPlay")];
    }

    public static void Postfix(CardModel __instance, ref PileType __result)
    {
        if (__result == PileType.Discard && Helper.HasCustomDynamic(__instance, "Hysteresis")){
            __result = PileType.Hand;
        }
    }
}