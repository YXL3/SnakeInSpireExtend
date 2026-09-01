using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Cards;

namespace SnakeInSpireExtend.Scripts.Powers;

public class IneluctablePower : SnakePowerTemplate
{    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, Amount);
        foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, player, prefs, null, this))
        {
            CardModel cardModel2 = CombatState.CreateCard<Ineluctable>(player);
            await CardCmd.Transform(item, cardModel2);
        }
    }
}