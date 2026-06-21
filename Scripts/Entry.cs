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
namespace SnakeInSpireExtend.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{ 
    public const string ModId = "SnakeInSpireExtend";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);
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
        if (!patcher.PatchAll())
            throw new InvalidOperationException("Critical patches failed.");
        SnakeModRewardRegister.TransformRegister();
    }
}