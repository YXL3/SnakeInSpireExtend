using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using SnakeInSpireExtend.Scripts.Cards;
using SnakeInSpireExtend.Scripts.Extension.Patch;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Patching.Core;
using SnakeInSpireExtend.Scripts.Rewards;
using SnakeInSpireExtend.Scripts.Relics;
using STS2RitsuLib.Scaffolding.Cards.HandOutline;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.RunData;

namespace SnakeInSpireExtend.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{ 
    public const string ModId = "SnakeInSpireExtend";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);
    private const string SneakyPhantomSavedDataKey = "sneaky_phantom_carry_over";
    public static PlayerRunSavedData<PhantomCarryOverState> SneakyPhantomSavedData = null!;
    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<FlashOfFang, Gleam>();
        RitsuLibFramework.RegisterTouchOfOrobasRefinementMapping<SnakeEye, CalamityEye>();

        var patcher = RitsuLibFramework.CreatePatcher(ModId, "patches");
        patcher.RegisterPatch<HysteresisFunctionPatch>();
        patcher.RegisterPatch<GetDescriptionForPilePatch>();
        patcher.RegisterPatch<HoverTipsPatch>();
        patcher.RegisterPatch<KeywordsPatch>();
        patcher.RegisterPatch<KillerMovePatch>();
        patcher.RegisterPatch<SnakeCardPortraitFilterPatch>();
        if (!patcher.PatchAll())
            throw new InvalidOperationException("Critical patches failed.");
        SnakeModRewardRegister.TransformRegister();

        ModCardHandOutlineRegistry.Register<CardModel>(ModCardHandOutlineRules.Dynamic(
            card => card.CanPlay() && !(card.ShouldGlowGold || card.ShouldGlowRed)
            &&(Helper.HasCustomDynamic(card, "Hysteresis") || Helper.HasCustomDynamic(card, "Haste")),
            card => Helper.HasCustomDynamic(card, "Hysteresis")?(Helper.HasCustomDynamic(card, "Haste")
            ? Godot.Colors.Snow : Godot.Colors.Purple): Godot.Colors.Green
        ));

        SneakyPhantomSavedData = RunSavedDataStore.For(ModId).RegisterPerPlayer<PhantomCarryOverState>(
            SneakyPhantomSavedDataKey, () => new(), new() { WritePolicy = RunSavedDataWritePolicy.WhenNonDefault});

    }
}