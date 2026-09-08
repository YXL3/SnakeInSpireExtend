using SnakeInSpireExtend.Scripts.RelicPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Relics;

[RegisterRelic(typeof(SnakeRelicPool), Inherit = true)]
public abstract class SnakeRelicTemplate : ModRelicTemplate
{
    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png"
    );
}