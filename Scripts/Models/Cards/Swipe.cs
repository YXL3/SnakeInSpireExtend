using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class Swipe() : ModCardTemplate(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("HysteresisVar",1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw){
        if (card != this)
        {
            return Task.CompletedTask;
        }
        Helper.Hysteresis(card, DynamicVars["HysteresisVar"].BaseValue);
        return Task.CompletedTask;
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [..Helper.HysteresisHoverTipIfNeeded(this)];
}