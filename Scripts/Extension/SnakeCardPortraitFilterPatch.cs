using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SnakeInSpireExtend.Scripts.Cards;
using STS2RitsuLib.Patching.Models;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class SnakeCardPortraitFilterPatch : IPatchMethod
{
    public static string PatchId => "snake_card_portrait_filter";

    public static string Description => "Sets nearest-neighbor texture filtering on portrait nodes for Snake card inheritors.";

    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(NCard), "UpdatePortrait")];
    }

    public static void Postfix(NCard __instance)
    {
        if (__instance.Model is not SnakeCardTemplate)
        {
            return;
        }

        TextureRect portrait = Traverse.Create(__instance).Field("_portrait").GetValue<TextureRect>();
        if (portrait != null)
        {
            portrait.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        }

        TextureRect ancientPortrait = Traverse.Create(__instance).Field("_ancientPortrait").GetValue<TextureRect>();
        if (ancientPortrait != null)
        {
            ancientPortrait.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
        }
    }
}