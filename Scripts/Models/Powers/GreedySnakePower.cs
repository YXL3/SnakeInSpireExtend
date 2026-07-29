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
        if (!participants.Contains(Owner))
        {
            return;
        }
        Flash();
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner.Player, prefs, null, this)).FirstOrDefault();      
        if (card != null)
        {
            await CardCmd.Exhaust(choiceContext, card);
            decimal amount = Helper.HasCustomDynamic(card, "Haste") ? card.DynamicVars["Haste"].BaseValue : 0;
            amount += Helper.HasCustomDynamic(card, "Hysteresis") ? card.DynamicVars["Hysteresis"].BaseValue : 0;
            amount *= Amount;
            await PowerCmd.Apply<PlatingPower>(choiceContext, Owner, amount, Owner, null);
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner, amount, Owner, null);
            await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null);
        }
    }
}
