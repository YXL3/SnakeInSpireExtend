using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Gleam() : SnakeCardTemplate(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(2),
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
#if STS2_AT_LEAST_0_111_0
            .FromCard(this, cardPlay)
#else
            .FromCard(this)
#endif
            .Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitVfxNode(t => NThinSliceVfx.Create(cardPlay.Target))
            .Execute(choiceContext);
        if(Helper.HasCustomDynamic(this, "Haste"))
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

#if STS2_AT_LEAST_0_111_0
    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation result = base.GetResultLocationForCardPlay();
        return (result.pileType == PileType.None || !Helper.HasCustomDynamic(this, "Haste"))?
        result : new CardLocation(Owner, PileType.Hand, CardPilePosition.Bottom);
    }
#else
    protected override PileType GetResultPileTypeForCardPlay()
    {
        PileType result = base.GetResultPileTypeForCardPlay();
        return (result == PileType.None || !Helper.HasCustomDynamic(this, "Haste"))? result : PileType.Hand;
    }
#endif

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        EnergyHoverTip,
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}