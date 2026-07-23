using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class Bite() : ModCardTemplate(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(15m, ValueProp.Move),
        new CalculationBaseVar(1),
        new CalculationExtraVar(1),
        new CalculatedVar("CalculatedHits").WithMultiplier((CardModel card, Creature? _) => CalculateHysteresisCount(card))
    ];

    private static int CalculateHysteresisCount(CardModel card)
    {
        if (!Helper.HasCustomDynamic(card,"Hysteresis"))
        {
            return 0;
        }
        return (int)card.DynamicVars["Hysteresis"].BaseValue;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target))
            .WithHitFx("vfx/vfx_bite")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(3m);
        AddKeyword(CardKeyword.Retain);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [..Helper.HysteresisHoverTipIfNeeded(this)];
}