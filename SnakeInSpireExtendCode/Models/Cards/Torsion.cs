using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Torsion() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal vigorLoss = (await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
#if STS2_AT_LEAST_0_111_0
            .FromCard(this, cardPlay)
#else
            .FromCard(this)
#endif
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_scratch")
            .Execute(choiceContext))
            .Results.SelectMany((List<DamageResult> r) => r).Sum((DamageResult r) => r.TotalDamage);
        await PowerCmd.Apply<VigorPower>(choiceContext, cardPlay.Target, -vigorLoss, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<VigorPower>()];
}