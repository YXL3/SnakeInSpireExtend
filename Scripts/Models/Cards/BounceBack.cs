using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class BounceBack() : SnakeCardTemplate(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HysteresisVar", 1m),
        new PowerVar<VigorPower>(15m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this))
        {
            Helper.Hysteresis(item, DynamicVars["HysteresisVar"].BaseValue);
        }
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, -DynamicVars["VigorPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HysteresisVar"].UpgradeValueBy(1m);
    }
    
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HysteresisHoverTipIfNeeded(this),
        HoverTipFactory.FromPower<VigorPower>()
    ];
}
