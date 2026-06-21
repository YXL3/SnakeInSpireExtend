using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
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
    // public override PowerAssetProfile AssetProfile => new(
    //     IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
    //     BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    // );
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner))
        {
            return;
        }
        Flash();
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        CardModel card = (await CardSelectCmd.FromHand(choiceContext, base.Owner.Player, prefs, null, this)).FirstOrDefault();      
        if (card != null)
        {
            await CardCmd.Exhaust(choiceContext, card);
            decimal amount = Helper.HasCustomDynamic(card, "Haste") ? card.DynamicVars["Haste"].BaseValue : 0;
            amount += Helper.HasCustomDynamic(card, "Hysteresis") ? card.DynamicVars["Hysteresis"].BaseValue : 0;
            amount *= base.Amount;
            await PowerCmd.Apply<PlatingPower>(choiceContext, base.Owner, amount, base.Owner, null);
            await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner, amount, base.Owner, null);
            await CreatureCmd.GainBlock(base.Owner, amount, ValueProp.Unpowered, null);
        }
    }
}
