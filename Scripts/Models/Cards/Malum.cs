using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Malum() : SnakeCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public interface IChoosable
    {
        public Task OnChosen(PlayerChoiceContext choiceContext, CardPlay cardPlay);
    }

    private static readonly IReadOnlyList<IChoosable> taboos = [ModelDb.Card<Blind>(), ModelDb.Card<Dread>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>(2m),
        new PowerVar<VulnerablePower>(2m),
        new DynamicVar("Turns", 2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        List<CardModel> cards = taboos.Select(c => CombatState.CreateCard((CardModel)c, Owner)).ToList();
        CardModel? cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, Owner);
        if(cardModel != null){
            await ((IChoosable)cardModel).OnChosen(choiceContext, cardPlay);
        }
        await PowerCmd.Apply<MalumPower>(choiceContext, Owner.Creature, DynamicVars["Turns"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
}

[RegisterCard(typeof(StatusCardPool))]
public class Dread() : ModCardTemplate(-1, CardType.Status, CardRarity.Status, TargetType.None, false), Malum.IChoosable
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VulnerablePower>(2m)];

    public override int MaxUpgradeLevel => 0;

    public override bool CanBeGeneratedInCombat => false;

    public async Task OnChosen(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, cardPlay.Card);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
}

[RegisterCard(typeof(StatusCardPool))]
public class Blind() : ModCardTemplate(-1, CardType.Status, CardRarity.Status, TargetType.None, false), Malum.IChoosable
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>(2m)];

    public override int MaxUpgradeLevel => 0;

    public override bool CanBeGeneratedInCombat => false;

    public async Task OnChosen(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars.Weak.BaseValue, Owner.Creature, cardPlay.Card);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<WeakPower>()];
}