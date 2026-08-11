using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Patching.Models;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class KillerMovePatch : IPatchMethod
{
    public static string PatchId => "killer_move_function";

    public static string Description => "Killer Move function";

    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(AttackCommand), nameof(AttackCommand.Execute))];
    }

    public static void Prefix(AttackCommand __instance)
    {
        if(__instance.Attacker == null || (!__instance.Attacker.HasPower<KillerMovePower>()) || __instance.ModelSource is not CardModel card)
        {
            return;
        }
        int num = CombatManager.Instance.History.CardPlaysStarted.Count((CardPlayStartedEntry e) => e.HappenedThisTurn(card.CombatState) && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Player == card.Owner);
        CardPile? pile = card.Pile;
        int num2 = (pile != null && pile.Type == PileType.Play) ? 1 : 0;
        if (num > num2)
        {
            return;
        }
        int hitCount = Traverse.Create(__instance).Field("_hitCount").GetValue<int>();
        hitCount += __instance.Attacker.GetPower<KillerMovePower>()!.Amount;
        __instance.WithHitCount(hitCount);
    }
}