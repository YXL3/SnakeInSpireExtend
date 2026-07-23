using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Extension;

public abstract class DualEffectCardTemplate(
    int energyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true)
    : ModCardTemplate(energyCost, type, rarity, targetType, shouldShowInCardLibrary){
    protected override PileType GetResultPileTypeForCardPlay()
    {
        PileType result = base.GetResultPileTypeForCardPlay();
        // return (result != PileType.Discard || !Helper.HasCustomDynamic(this, "Haste"))? result : PileType.Hand;
        return (result == PileType.None || !Helper.HasCustomDynamic(this, "Haste"))? result : PileType.Hand;
    }
    
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}


