using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class FrozenLenPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );

    private class Data
    {
        public readonly HashSet<CardModel> autoPlayingCards = new HashSet<CardModel>();
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (fromHandDraw && card.Owner == Owner.Player && card.Keywords.Contains(SnakeInSpireExtendCardKeywords.Keen))
        {
            GetInternalData<Data>().autoPlayingCards.Add(card);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        if (GetInternalData<Data>().autoPlayingCards.Any())
        {
            Flash();
            foreach (CardModel card in GetInternalData<Data>().autoPlayingCards)
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
            GetInternalData<Data>().autoPlayingCards.Clear();
        }
        await PowerCmd.Decrement(this);
    }
}
