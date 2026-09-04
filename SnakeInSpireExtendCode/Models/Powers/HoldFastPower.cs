using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SnakeInSpireExtend.Scripts.Powers;

public class HoldFastPower : SnakePowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        if (card.Owner == Owner.Player && (card.Type == CardType.Attack || card.Type == CardType.Skill))
        {
            card.AddKeyword(CardKeyword.Retain);
            await CardPileCmd.Add(card, PileType.Hand);
            await PowerCmd.Decrement(this);
        }
    }
}