using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class LastStandPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }
        List<CardModel> items = PileType.Hand.GetPile(Owner.Player).Cards.Where((CardModel c) => Helper.HasCustomDynamic(c, "Hysteresis")).ToList();
        if (items.Count == 0)
        {
            return;
        }
        decimal maxValue = items.Max(c => c.DynamicVars["Hysteresis"].BaseValue);
        items = items.Where(c => c.DynamicVars["Hysteresis"].BaseValue == maxValue).ToList();
        CardModel? card = Owner.Player.RunState.Rng.Shuffle.NextItem(items);
        if (card != null)
        {
            Flash();
            GetInternalData<Data>().playingCard = card;
            await CardCmd.AutoPlay(choiceContext, card, null);
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    private class Data
    {
        public CardModel? playingCard;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card != GetInternalData<Data>().playingCard)
        {
            return playCount;
        }
        return (int)(Helper.HasCustomDynamic(card, "Hysteresis")? playCount + card.DynamicVars["Hysteresis"].BaseValue - 1 : playCount);
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        GetInternalData<Data>().playingCard = null;
    }

}
