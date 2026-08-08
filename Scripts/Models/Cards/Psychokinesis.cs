using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Powers;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Psychokinesis() : SnakeCardTemplate(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PsychokinesisPower>(1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PsychokinesisPower>(choiceContext, Owner.Creature, DynamicVars["PsychokinesisPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [..Helper.HasteHoverTipIfNeeded(this)];
}