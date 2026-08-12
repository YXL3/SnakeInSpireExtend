using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCharacterStarterCard(typeof(Snake), 1, Order = 2)]
public class FlashOfFang() : SnakeCardTemplate(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitVfxNode(t => NThinSliceVfx.Create(cardPlay.Target))
            .Execute(choiceContext);
    }
    
    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation result = base.GetResultLocationForCardPlay();
        return (result.pileType == PileType.None || !Helper.HasCustomDynamic(this, "Haste"))?
        result : new CardLocation(Owner, PileType.Hand, CardPilePosition.Bottom);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}


