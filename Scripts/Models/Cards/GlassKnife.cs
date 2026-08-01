using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeInSpireExtend.Scripts.Cards;

public class GlassKnife() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override int MaxUpgradeLevel => int.MaxValue;

    private int remainingPlays = 5;

    [SavedProperty]
    public int RemainingPlays
    {
        get
        {
            return remainingPlays;
        }
        set
        {
            AssertMutable();
            remainingPlays = value;
            DynamicVars["Plays"].BaseValue = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar("Plays", 5m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitVfxNode(t => NThinSliceVfx.Create(cardPlay.Target))
            .Execute(choiceContext);
        if(DeckVersion is GlassKnife deckVersion)
        {
            RemainingPlays--;
            deckVersion.RemainingPlays--;
            if (deckVersion.RemainingPlays < 0)
            {
                await CardPileCmd.RemoveFromDeck(deckVersion);
            }
        }
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(3m);
        RemainingPlays += 3;
    }
}