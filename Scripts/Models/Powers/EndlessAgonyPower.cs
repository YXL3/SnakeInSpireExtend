using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SnakeInSpireExtend.Scripts.Powers;

public class EndlessAgonyPower : SnakePowerTemplate
{
    private class Data
    {
        public CardModel? selectedCard;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Card")];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        CardModel? card = GetInternalData<Data>().selectedCard;
        if (player == Owner.Player && card != null && card.Pile != null && (card.Pile.Type == PileType.Draw || card.Pile.Type == PileType.Discard))
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    public void SetSelectedCard(CardModel card)
    {
        GetInternalData<Data>().selectedCard = card;
        ((StringVar)DynamicVars["Card"]).StringValue = card.Title;
    }
}