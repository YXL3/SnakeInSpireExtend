using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Lurk() : SnakeCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>(2m),
        new DynamicVar("HasteVar", 1m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(CombatState == null) return;
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, DynamicVars.Weak.BaseValue, Owner.Creature, this);
        }
        foreach(CardModel card in PileType.Hand.GetPile(Owner).Cards.Where(card => card.Type == CardType.Skill))
        {
            Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
    
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<WeakPower>(),
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}