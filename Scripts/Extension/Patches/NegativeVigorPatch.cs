using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Patching.Models;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class NegativeVigorPatch : IPatchMethod
{
    public static string PatchId => "negative_vigor";

    public static string Description => "Patch that override VigorPower's AllowNegative to true.";

    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(VigorPower), "get_AllowNegative", MethodType.Normal)];
    }

    public static void Postfix(ref bool __result)
    {
        __result = true;
    }
}