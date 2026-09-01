using MegaCrit.Sts2.Core.Entities.Cards;
using SnakeInSpireExtend.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool), Inherit = true)]
public abstract class SnakeCardTemplate(
    int energyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true)
    : ModCardTemplate(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
{
}