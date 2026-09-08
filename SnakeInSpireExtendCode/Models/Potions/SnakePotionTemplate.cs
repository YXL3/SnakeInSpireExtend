using SnakeInSpireExtend.Scripts.PotionPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Potions;

[RegisterPotion(typeof(SnakePotionPool), Inherit = true)]
public abstract class SnakePotionTemplate : ModPotionTemplate
{
    public override PotionAssetProfile AssetProfile => new(
        ImagePath: $"res://SnakeInSpireExtend/images/potions/{GetType().Name}.png",
        OutlinePath: $"res://SnakeInSpireExtend/images/potions/{GetType().Name}.png"
    );
}