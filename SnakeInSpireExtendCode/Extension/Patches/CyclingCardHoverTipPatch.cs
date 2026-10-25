using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using SnakeInSpireExtend.Scripts.Extension.HoverTips;
using STS2RitsuLib.Patching.Models;
using Timer = Godot.Timer;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

/// <summary>
/// Extends the vanilla card-hover-tip container with support for a single card slot
/// whose displayed model rotates through <see cref="CyclingCardHoverTip.Tips"/>.
/// </summary>
public class CyclingCardHoverTipPatch : IPatchMethod
{
    private const string CardHoverTipScenePath = "res://scenes/ui/card_hover_tip.tscn";

    public static string PatchId => "cycling_card_hover_tip";

    public static string Description => "Renders CyclingCardHoverTip as a timed card-preview carousel.";

    public static bool IsCritical => true;

    public static ModPatchTarget[] GetTargets()
    {
        return
        [
            new(typeof(NHoverTipCardContainer), nameof(NHoverTipCardContainer.Add), [typeof(CardHoverTip)])
        ];
    }

    public static bool Prefix(NHoverTipCardContainer __instance, CardHoverTip cardTip)
    {
        if (cardTip is not CyclingCardHoverTip cyclingTip)
        {
            return true;
        }

        CreateCarousel(__instance, cyclingTip);
        return false;
    }

    private static void CreateCarousel(NHoverTipCardContainer container, CyclingCardHoverTip cyclingTip)
    {
        Control tipControl = PreloadManager.Cache
            .GetScene(CardHoverTipScenePath)
            .Instantiate<Control>(PackedScene.GenEditState.Disabled);

        container.AddChildSafely(tipControl);

        NCard cardNode = tipControl.GetNode<NCard>("%Card");
        int index = 0;

        void ApplyCurrentTip()
        {
            CardHoverTip currentTip = cyclingTip.Tips[index];
            cardNode.Model = currentTip.Card;
            cardNode.UpdateVisuals(PileType.Deck, CardPreviewMode.Normal);
        }

        ApplyCurrentTip();

        Timer timer = new()
        {
            WaitTime = cyclingTip.IntervalSeconds,
            OneShot = false,
            Autostart = true,
            ProcessMode = Node.ProcessModeEnum.Always,
        };

        timer.Timeout += () =>
        {
            if (!GodotObject.IsInstanceValid(cardNode))
            {
                return;
            }

            index = (index + 1) % cyclingTip.Tips.Count;
            ApplyCurrentTip();
        };

        // The timer is owned by the hover-tip container, so it is freed together
        // with the hover tip when NHoverTipSet.Remove(owner) removes the set.
        container.AddChildSafely(timer);
    }
}
