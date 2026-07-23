using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class Defender() : ModCardTemplate(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7m, ValueProp.Move),
        new RepeatVar(2),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}