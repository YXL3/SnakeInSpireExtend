using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class MalumPower : ModPowerTemplate
{
    // public override PowerAssetProfile AssetProfile => new(
    //     IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
    //     BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    // );

    private Malum? malum;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(cardSource != null && cardSource is Malum)
        {
            malum = (Malum?)cardSource;
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
        if(malum != null && malum.IsMutable)
        {
            await CardCmd.AutoPlay(choiceContext, malum, null);
        }
        await PowerCmd.Remove(this);
    }
}
