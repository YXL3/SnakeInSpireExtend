using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeInSpireExtend.Scripts.Powers;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Glider() : SnakeCardTemplate(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GliderPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<SoarPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<SoarPower>()
    ];
}