using System.Reflection;
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

    private static FieldInfo _portraitField = AccessTools.Field(typeof(NCard), "_portrait");

    public static void Postfix(NCard __instance)
    {
        if (_portraitField.GetValue(__instance) is not TextureRect portrait) return;
        portrait.TextureFilter = __instance.Model is SnakeCardTemplate ? CanvasItem.TextureFilterEnum.Nearest : CanvasItem.TextureFilterEnum.Linear;
    }
}