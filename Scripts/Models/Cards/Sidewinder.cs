using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Sidewinder() : SnakeCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
        new DynamicVar("HysteresisVar", 1m),
        new DynamicVar("HasteVar", 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this))
        {
            Helper.Hysteresis(item, DynamicVars["HysteresisVar"].BaseValue);
            if (IsUpgraded)
            {
                Helper.Haste(item, DynamicVars["HasteVar"].BaseValue);
            }
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => IsUpgraded
        ? [..Helper.HysteresisHoverTipIfNeeded(this), ..Helper.HasteHoverTipIfNeeded(this)]
        : [..Helper.HysteresisHoverTipIfNeeded(this)];
}