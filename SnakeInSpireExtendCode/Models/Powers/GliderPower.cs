using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeInSpireExtend.Scripts.Powers;

public class GliderPower : SnakePowerTemplate
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        Flash();
        if(Owner.GetPower<SoarPower>() != null)
        {
            await PowerCmd.Remove<SoarPower>(Owner);
        }
        else
        {
            await PowerCmd.Apply<SoarPower>(choiceContext, Owner, 1m, null, null);
        }
    }
}