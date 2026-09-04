using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Cards;

namespace SnakeInSpireExtend.Scripts.Powers;

public class MalumPower : SnakePowerTemplate
{
    private class Data
    {
        public Malum? malum;
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(cardSource != null && cardSource is Malum)
        {
            GetInternalData<Data>().malum = (Malum?)cardSource;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        if (Amount > 1)
        {
            await PowerCmd.Decrement(this);
            return;
        }
        Malum? malum = GetInternalData<Data>().malum;
        if(malum != null && malum.IsMutable)
        {
            malum = (Malum)malum.CreateClone();
            CardCmd.ClearAffliction(malum);
            await CardCmd.AutoPlay(choiceContext, malum, null);
        }
        await PowerCmd.Remove(this);
    }
}