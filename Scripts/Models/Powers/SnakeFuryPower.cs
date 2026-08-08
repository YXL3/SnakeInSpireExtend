using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class SnakeFuryPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private decimal _vigorAmountBeforeAttack;

    public override async Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker == Owner && command.DamageProps.IsPoweredAttack() && command.ModelSource is CardModel)
        {
            if (Owner.HasPower<VigorPower>())
            {
                _vigorAmountBeforeAttack = Owner.GetPower<VigorPower>()!.Amount;
            }
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player && _vigorAmountBeforeAttack > 0)
        {
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner, _vigorAmountBeforeAttack, Owner, null);
            await PowerCmd.Decrement(this);
            _vigorAmountBeforeAttack = 0;
        }
    }
}
