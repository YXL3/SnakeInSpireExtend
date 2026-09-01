using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace SnakeInSpireExtend.Scripts.Powers;

public class SneakyPhantomPower : SnakePowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("Cards"),
        new EnergyVar(0)
    ];

    public void SetProperty(IEnumerable<CardModel> cards, int energy)
    {
        ((StringVar)DynamicVars["Cards"]).StringValue = string.Join(", ", cards.Select(c => c.Title));
        DynamicVars.Energy.BaseValue = energy;    
    }
}