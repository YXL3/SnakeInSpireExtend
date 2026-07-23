using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class TracerShotPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == Owner && PileType.Hand.GetPile(card.Owner).Cards.Count >= CardPile.MaxCardsInHand)
        {
            await PlayerCmd.GainEnergy(Amount, card.Owner);
            await PowerCmd.Remove(this);
        }
    }
}
