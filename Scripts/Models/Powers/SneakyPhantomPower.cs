using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class SneakyPhantomPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("Cards"),
        new BlockVar(0m, ValueProp.Unpowered),
        new EnergyVar(0)
    ];

    public void SetProperty(IEnumerable<CardModel> cards, int block, int energy)
    {
        ((StringVar)DynamicVars["Cards"]).StringValue = string.Join(", ", cards.Select(c => c.Title));
        DynamicVars.Block.BaseValue = block;
        DynamicVars.Energy.BaseValue = energy;    
    }
}
