using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class Sidewinder : ModCardTemplate
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
        new DynamicVar("HysteresisVar", 1m),
        new DynamicVar("HasteVar", 1m),
    ];

    public Sidewinder(): base(1, CardType.Skill, CardRarity.Common, TargetType.Self){}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 1), null, this))
        {
            Helper.Hysteresis(item, base.DynamicVars["HysteresisVar"].BaseValue);
            if (IsUpgraded)
            {
                Helper.Haste(item, base.DynamicVars["HasteVar"].BaseValue);
            }
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => base.IsUpgraded
        ? [Helper.HysteresisHoverTip(), Helper.HasteHoverTip(this)]
        : [Helper.HysteresisHoverTip()];
}