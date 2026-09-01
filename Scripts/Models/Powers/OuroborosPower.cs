using MegaCrit.Sts2.Core.Entities.Powers;

namespace SnakeInSpireExtend.Scripts.Powers;

public class OuroborosPower : SnakePowerTemplate
{    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
}