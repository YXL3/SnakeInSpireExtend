using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class Gleam() : DualEffectCardTemplate(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move),
        new RepeatVar(3),
        new BlockVar(3m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitVfxNode((Creature t) => NStabVfx.Create(t, facingEnemies: true))
            .WithHitFx(null, null, "blunt_attack.mp3")
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Repeat.UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}


