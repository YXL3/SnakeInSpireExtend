using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Models;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class SnakeFury() : ModCardTemplate(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VigorPower>(3m),
        new PowerVar<SnakeFuryPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, DynamicVars["VigorPower"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<SnakeFuryPower>(choiceContext, Owner.Creature, DynamicVars["SnakeFuryPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VigorPower"].UpgradeValueBy(1m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<VigorPower>(),
    ];
}