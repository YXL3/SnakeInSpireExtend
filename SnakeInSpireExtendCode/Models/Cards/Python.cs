using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Random;
using SnakeInSpireExtend.Scripts.Extension.HoverTips;
using static MegaCrit.Sts2.Core.Models.Events.TinkerTime;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Python() : SnakeCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(CombatState == null) return;
        Dictionary<CardType, IReadOnlyList<RiderEffect>> randomRiders = GetRandomRiders();
        List<CardModel> cards = randomRiders.Keys.Select(cardType => GetMadScience(cardType, RiderEffect.None)).ToList();
        CardModel? card = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner);
        if(card == null) return;
        CardType chosenType = card.Type;
        IReadOnlyList<RiderEffect> riders = randomRiders[chosenType];
        cards = riders.Select(rider => GetMadScience(chosenType, rider)).ToList();
        card = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner);
        if(card == null) return;
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }

    private static readonly Dictionary<CardType, IReadOnlyList<RiderEffect>> riderTable = new()
    {
        [CardType.Attack] = [RiderEffect.Sapping, RiderEffect.Violence, RiderEffect.Choking],
        [CardType.Skill] = [RiderEffect.Energized, RiderEffect.Wisdom, RiderEffect.Chaos],
        [CardType.Power] = [RiderEffect.Expertise, RiderEffect.Curious, RiderEffect.Improvement]
    };

    private bool CanGenerateImprovement => CombatManager.Instance.History.Entries.OfType<CardGeneratedEntry>()
    .Any(e => e.Creator == Owner && e.Card is MadScience m && m.TinkerTimeType == CardType.Power && m.TinkerTimeRider == RiderEffect.Improvement)
    && DeckVersion != null;

    private Dictionary<CardType, IReadOnlyList<RiderEffect>> GetRandomRiders()
    {
        Rng rng = Owner.RunState.Rng.CombatCardGeneration;
        List<CardType> selectedCardTypes = riderTable.Keys.TakeRandom(2, rng).ToList();
        Dictionary<CardType, IReadOnlyList<RiderEffect>> result = new Dictionary<CardType, IReadOnlyList<RiderEffect>>(selectedCardTypes.Count);
        foreach (CardType cardType in selectedCardTypes)
        {
            IEnumerable<RiderEffect> pool = riderTable[cardType];
            if (cardType == CardType.Power && CanGenerateImprovement)
            {
                pool = pool.Where(effect => effect != RiderEffect.Improvement);
            }
            result[cardType] = pool.TakeRandom(2, rng).ToList();
        }
        return result;
    }

    private CardModel GetMadScience(CardType cardType, RiderEffect rider, bool forPreview = false)
    {
        MadScience madScience = forPreview
            ? (MadScience)ModelDb.Card<MadScience>().MutableClone()
            : CombatState!.CreateCard<MadScience>(Owner);
        madScience.TinkerTimeType = cardType;
        madScience.TinkerTimeRider = rider;
        return madScience;
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips
    {
        get
        {
            CardModel[] previewCards =
            [
                GetMadScience(CardType.Attack, RiderEffect.None, forPreview: true),
                GetMadScience(CardType.Skill, RiderEffect.None, forPreview: true),
                GetMadScience(CardType.Power, RiderEffect.None, forPreview: true)
            ];
            return [new CyclingCardHoverTip(previewCards)];
        }
    }
}