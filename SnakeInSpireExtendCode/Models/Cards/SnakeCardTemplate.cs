using Godot;
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
    public override CardAssetProfile AssetProfile
    {
        get
        {
            string portraitPath = $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png";
            Material? portraitMaterial = Rarity == CardRarity.Ancient ? null : SnakePortraitMaterialCache.Get(portraitPath);
            return new CardAssetProfile(
                PortraitPath: portraitPath,
                PortraitMaterial: portraitMaterial
            );
        }
    }
}
