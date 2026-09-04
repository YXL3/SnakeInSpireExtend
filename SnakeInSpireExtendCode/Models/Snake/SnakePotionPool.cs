using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.PotionPools;

public class SnakePotionPool : TypeListPotionPoolModel
{
    public override string? TextEnergyIconPath => "res://SnakeInSpireExtend/images/snake/energy_snake.png";

    public override string? BigEnergyIconPath => "res://SnakeInSpireExtend/images/snake/energy_snake_big.png";

    public override string EnergyColorName => "snake";
}