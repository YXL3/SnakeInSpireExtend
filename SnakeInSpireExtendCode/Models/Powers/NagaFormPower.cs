using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Enchantment;

namespace SnakeInSpireExtend.Scripts.Powers;

public class NagaFormPower : SnakePowerTemplate
{    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || Owner.Player == null)
        {
            return;
        }
        CardModel? card = PileType.Draw.GetPile(Owner.Player).Cards.FirstOrDefault();
        if(card != null && ModelDb.Enchantment<Cataclysm>().CanEnchant(card))
        {
            CardCmd.Enchant<Cataclysm>(card, 1m);
        }
    }
}