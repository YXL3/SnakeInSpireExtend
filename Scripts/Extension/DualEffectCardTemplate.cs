using MegaCrit.Sts2.Core.Entities.Cards;
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
        if (result != PileType.Discard || !Helper.HasCustomDynamic(this, "Haste"))return result;
        return PileType.Hand;
    }
}


