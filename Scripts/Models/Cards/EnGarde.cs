using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class EnGarde() : ModCardTemplate(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SnakeInSpireExtendCardKeywords.Keen];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        EnergyHoverTip
    ];
}