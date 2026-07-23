using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class KillerMove() : ModCardTemplate(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(11m, ValueProp.Move),
        new EnergyVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
        await PlayerCmd.LoseEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(4m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        EnergyHoverTip
    ];
}