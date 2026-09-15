using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SnakeInSpireExtend.Scripts.Cards;

namespace SnakeInSpireExtend.Scripts.Relics;

public class PlantSac : SnakeRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player == Owner)
        {
            Flash();
            await CardPileCmd.AddGeneratedCardToCombat(combatState.CreateCard<Predate>(Owner), PileType.Hand, Owner);
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<Predate>()
    ];
}