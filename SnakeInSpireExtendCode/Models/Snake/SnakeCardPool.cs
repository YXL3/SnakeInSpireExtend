using Godot;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace SnakeInSpireExtend.Scripts.CardPools;

public class SnakeCardPool : TypeListCardPoolModel, IModColorfulPhilosophersCardPool
{
    public override string Title => "snake";

    public override string EnergyColorName => "snake";

    public override string? TextEnergyIconPath => "res://SnakeInSpireExtend/images/snake/energy_snake.png";

    public override string? BigEnergyIconPath => "res://SnakeInSpireExtend/images/snake/energy_snake_big.png";

    public override Color DeckEntryCardColor => new(0.5f, 1f, 0f);

    public override Color EnergyOutlineColor => new(0f, 0f, 0f);

    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateReplaceHueShaderMaterial(0.5f, 1f, 0f);

    public override Material? PoolFrameMaterial => _poolFrameMaterial;

    public override bool IsColorless => false;
}