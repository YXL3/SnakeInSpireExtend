using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class LethalStripesPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );

    private class Data
    {
        public required HashSet<CardModel> recordedCards;
    }
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData()
    {
        return new Data()
        {
            recordedCards = new HashSet<CardModel>()
        };
    }
    
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player && !Helper.HasCustomDynamic(cardPlay.Card, "Haste"))
        {
            GetInternalData<Data>().recordedCards.Add(cardPlay.Card);
        }
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player && GetInternalData<Data>().recordedCards.Contains(cardPlay.Card))
        {
            GetInternalData<Data>().recordedCards.Remove(cardPlay.Card);
            Flash();
            List<CardModel> cards = CardFactory.GetDistinctForCombat(Owner.Player, from c in Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint)
                                                                    where c.Rarity == CardRarity.Common
                                                                    select c, Amount, Owner.Player.RunState.Rng.CombatCardGeneration).ToList();
            CardCmd.Upgrade(cards, CardPreviewStyle.None);
            foreach(CardModel item in cards)
            {
                item.AddKeyword(SnakeInSpireExtendCardKeywords.Keen);
            }
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, Owner.Player, CardPilePosition.Top));
        }
    }
}
