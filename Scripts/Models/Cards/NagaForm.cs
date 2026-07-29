using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Powers;

namespace SnakeInSpireExtend.Scripts.Cards;

public class NagaForm() : SnakeCardTemplate(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> list = PileType.Hand.GetPile(Owner).Cards.Where(c => c.Type == CardType.Attack).ToList();
        if (!list.Any())
        {
            return;
        }
        foreach (CardModel item in list)
        {
            await CardCmd.Exhaust(choiceContext, item);
            CardCmd.ClearAffliction(item);
        }
        NagaFormPower? power = await PowerCmd.Apply<NagaFormPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if(power != null)
        {
            power.SetCards(list);
        }
    }

    protected override void OnUpgrade(){
        AddKeyword(CardKeyword.Retain);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
}