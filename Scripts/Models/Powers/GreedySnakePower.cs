using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class GreedySnakePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player == null)
        {
            return;
        }
        decimal amount = 0m;
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, Amount);
        foreach(CardModel card in await CardSelectCmd.FromHand(choiceContext, Owner.Player, prefs, null, this)){
            await CardCmd.Exhaust(choiceContext, card);
            amount += Helper.HasCustomDynamic(card, "Haste") ? card.DynamicVars["Haste"].BaseValue : 0;
            amount += Helper.HasCustomDynamic(card, "Hysteresis") ? card.DynamicVars["Hysteresis"].BaseValue : 0;
        }
        await PowerCmd.Apply<PlatingPower>(choiceContext, Owner, amount, Owner, null);
        await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null);
    }
}
