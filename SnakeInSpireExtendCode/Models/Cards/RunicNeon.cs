using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class RunicNeon() : SnakeCardTemplate(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HysteresisVar", 2m),
        new DynamicVar("HasteVar", 2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this)).FirstOrDefault();
        if(card != null)
        {
            Helper.Hysteresis(card, DynamicVars["HysteresisVar"].BaseValue);
            Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
            await CardCmd.Discard(choiceContext, PileType.Hand.GetPile(Owner).Cards.Where(c => c is not null && c != card).ToList());
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HysteresisVar"].UpgradeValueBy(1m);
        DynamicVars["HasteVar"].UpgradeValueBy(1m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HysteresisHoverTipIfNeeded(this),
        ..Helper.HasteHoverTipIfNeeded(this),
    ];
}