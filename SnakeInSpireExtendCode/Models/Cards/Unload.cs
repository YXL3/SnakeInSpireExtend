using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Powers;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Unload() : SnakeCardTemplate(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> list;
        if (IsUpgraded)
        {
            list = PileType.Hand.GetPile(Owner).Cards.Where(c => c.Type == CardType.Attack).ToList();
        }
        else
        {
            list = (List<CardModel>)await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), c => c.Type == CardType.Attack, this);
        }
        if (list.Count == 0)
        {
            return;
        }
        foreach (CardModel item in list)
        {
            await CardCmd.Exhaust(choiceContext, item);
            CardCmd.ClearAffliction(item);
        }
        UnloadPower? power = await PowerCmd.Apply<UnloadPower>(choiceContext, Owner.Creature, ResolveEnergyXValue(), Owner.Creature, this);
        if(power != null)
        {
            power.SetCards(list);
        }
    }
}