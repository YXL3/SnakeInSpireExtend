using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Cards;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Patching.Models;

namespace SnakeInSpireExtend.Scripts.Extension.Patch;

public class HasteFunctionPatch : IPatchMethod
{
    public static string PatchId => "haste_function";

    public static string Description => "Haste function";

    public static bool IsCritical => true;

    public static ModPatchTarget[] GetTargets()
    {
        // OnPlay 是 protected virtual，子类重写后 Harmony patch 不会对重写方法生效。
        // OnPlayWrapper 是 public 且非 virtual，所有卡牌打出时必定走这里。
        return [new(typeof(CardModel), "OnPlayWrapper")];
    }

    /// <summary>
    ///     Harmony Postfix on async methods fires when the Task is <em>returned</em> (first await),
    ///     not when it <em>completes</em>. This means DoHaste could run and clear Haste before
    ///     OnPlay has a chance to read it (e.g. TailWhip's HasteReplayCount).
    ///
    ///     To fix this, we capture the returned Task and schedule a continuation that runs
    ///     after OnPlayWrapper fully completes. This guarantees OnPlay has consumed Haste
    ///     before we try to clear or propagate it.
    ///
    ///     We replace __result (via ref) with the continuation Task so that the PlayCardAction
    ///     execution does not complete until all Haste processing finishes. This keeps
    ///     ActionExecutor.CurrentlyRunningAction valid throughout the auto-play chain,
    ///     preventing "Tried to interrupt shared queue action" errors when a Keen card's
    ///     OnPlay calls CardSelectCmd.FromHand.
    /// </summary>
    public static void Postfix(CardModel __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        // onPlayTask may be null if the async method threw synchronously before yielding
        Task onPlayTask = __result;
        if (onPlayTask != null)
        {
            __result = DoHasteAfterOnPlay(__instance, choiceContext, onPlayTask);
        }

        // Hysteresis decrement must also wait for OnPlay to finish,
        // so it's moved into the continuation below.
    }

    private static async Task DoHasteAfterOnPlay(CardModel card, PlayerChoiceContext choiceContext, Task onPlayTask)
    {
        // Wait for OnPlayWrapper to truly complete (all awaits, including OnPlay).
        // Do NOT catch exceptions here — let them propagate to the caller through
        // the replaced __result Task. If OnPlayWrapper threw, we naturally skip
        // Haste/Hysteresis processing because this method faults at the await.
        await onPlayTask;

        // OnPlay is fully done. It is now safe to read/clear Haste.
        await DoHaste(card, choiceContext);

        if (CardTypesOfDualEffect.Contains(card.GetType()))
        {
            await DoHaste(card, choiceContext, "Hysteresis");
        }

        if (card.DynamicVars.ContainsKey("Hysteresis") && card.DynamicVars["Hysteresis"].BaseValue > 0)
        {
            card.DynamicVars["Hysteresis"].UpgradeValueBy(-1m);
        }
    }
    
    private static readonly HashSet<Type> CardTypesOfDualEffect = new()
    {
        typeof(FlashOfFang),
        typeof(Gleam),
        typeof(ShaveTheGround)
    };

    private static async Task DoHaste(CardModel card, PlayerChoiceContext choiceContext, string functionalHaste = "Haste")
    {
        if (!Helper.HasCustomDynamic(card, functionalHaste))
        {
            return;
        }
        IEnumerable<CardModel> cards = await CardPileCmd.Draw(choiceContext, Helper.HasteDrawingAmount(card), card.Owner);
        if (!cards.Any())
        {
            return;
        }
        decimal snakeFury = Helper.GetOwnerPowerAmount<SnakeFuryPower>(card);
        if (functionalHaste == "Haste")
        {
            decimal hastePassed = Helper.HastePassingAmount(card);
            card.DynamicVars[functionalHaste].BaseValue = 0m;
            foreach (CardModel item in cards){
                if (item.Keywords.Contains(SnakeInSpireExtendCardKeywords.Keen) && item != card)
                {
                    // ICombatState? combatState = drawn.CombatState ?? drawn.Owner.Creature.CombatState;
                    // Creature? target = drawn.Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
                    // RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new PlayCardAction(drawn, target));
                    Helper.Haste(item, hastePassed);
                    if(snakeFury != 0)
                    {
                        item.GiveSingleTurnRetain();
                        Helper.Hysteresis(item, snakeFury);
                    }
                    else
                    {
                        await CardCmd.AutoPlay(choiceContext, item, null);
                    }
                    continue;
                }
                Helper.Haste(item, hastePassed);
            }
        }
    }
}