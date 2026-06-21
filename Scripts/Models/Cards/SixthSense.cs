using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class SixthSense : ModCardTemplate
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SnakeInSpireExtendCardKeywords.Keen];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new DynamicVar("HysteresisVar", 1m),
        new DynamicVar("HasteVar", 1m)
    ];

    public SixthSense(): base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self){}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (CardModel item in await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner))
        {
            Helper.Hysteresis(item, base.DynamicVars["HysteresisVar"].BaseValue);
            Helper.Haste(item, base.DynamicVars["HasteVar"].BaseValue);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars["HysteresisVar"].UpgradeValueBy(1m);
        base.DynamicVars["HasteVar"].UpgradeValueBy(1m);
    }
    
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        Helper.HysteresisHoverTip(),
        Helper.HasteHoverTip(this)
    ];
}