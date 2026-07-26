using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Garrotte() : SnakeCardTemplate(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9m, ValueProp.Move),
        new CalculationBaseVar(1),
        new CalculationExtraVar(1),
        new CalculatedVar("CalculatedHits").WithMultiplier((CardModel card, Creature? _) => CalculateOtherHasteHysteresisCount(card))
    ];

    private static int CalculateOtherHasteHysteresisCount(CardModel card)
    {
        int result = 0;
        foreach(CardModel handCard in PileType.Hand.GetPile(card.Owner).Cards)
        {
            if (handCard == card)
            {
                continue;
            }
            if (Helper.HasCustomDynamic(handCard,"Hysteresis"))
            {
                result += (int)handCard.DynamicVars["Hysteresis"].BaseValue;
            }
            if (Helper.HasCustomDynamic(handCard,"Haste"))
            {
                result += (int)handCard.DynamicVars["Haste"].BaseValue;
            }
        }
        return result;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target))
            .WithHitFx("vfx/vfx_chain")
            .OnlyPlayAnimOnce()
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
    
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HysteresisHoverTipIfNeeded(this),
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}