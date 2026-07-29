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
    protected override PileType GetResultPileTypeForCardPlay()
    {
        PileType result = base.GetResultPileTypeForCardPlay();
        return (result == PileType.None || !Helper.HasCustomDynamic(this, "Haste"))? result : PileType.Hand;
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}


