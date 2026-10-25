using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.RelicPools;

public class SnakeRelicPool : TypeListRelicPoolModel
{
    public override string? TextEnergyIconPath => "res://SnakeInSpireExtend/images/snake/energy_snake.png";

    public override string? BigEnergyIconPath => "res://SnakeInSpireExtend/images/snake/energy_snake_big.png";

    public override string EnergyColorName => "snake";

    public override Color LabOutlineColor => new(0.5f, 1f, 0f);
}