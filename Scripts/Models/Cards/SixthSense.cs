using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Models;

namespace SnakeInSpireExtend.Scripts.Cards;

public class SixthSense() : SnakeCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        SnakeInSpireExtendCardKeywords.Keen
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new DynamicVar("HysteresisVar", 1m),
        new DynamicVar("HasteVar", 1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (CardModel card in await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner))
        {
            Helper.Hysteresis(card, DynamicVars["HysteresisVar"].BaseValue);
            Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["HysteresisVar"].UpgradeValueBy(1m);
        DynamicVars["HasteVar"].UpgradeValueBy(1m);
    }
    
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HysteresisHoverTipIfNeeded(this),
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}