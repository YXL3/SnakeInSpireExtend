using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public abstract class DualEffectCardTemplate(
    int energyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true)
    : SnakeCardTemplate(energyCost, type, rarity, targetType, shouldShowInCardLibrary){
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


