using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Vfx;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Polaris() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7m, ValueProp.Move),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitVfxNode(t => NShootVfx.Create(t, VfxColor.Blue, bigBullet:true))
            .WithHitFx(null, null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (amount <= 0m ||
            cardPlay == null ||
            cardSource == null ||
            cardSource == this ||
            Pile?.Type != PileType.Hand ||
            cardPlay.Player != Owner ||
            cardSource.Type != CardType.Attack ||
            !props.IsPoweredAttack())
        {
            return 0m;
        }

        IRunState? runState = Owner.RunState;
        if (runState == null)
        {
            return 0m;
        }

        // The additive phase has already applied Strength and similar effects.
        // Resolve the remaining multiplicative/cap hooks to learn the damage that
        // the played attack would have dealt, then redirect that value into Polaris.
        decimal redirectedDamage = Hook.ModifyDamage(
            runState,
            CombatState,
            target,
            dealer,
            amount,
            props,
            cardSource,
            cardPlay,
            ModifyDamageHookType.Multiplicative | ModifyDamageHookType.Cap,
            CardPreviewMode.None,
            out _);

        AddDamage(redirectedDamage);
        return -amount;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    private decimal _extraDamage;

    private decimal ExtraDamage
    {
        get
        {
            return _extraDamage;
        }
        set
        {
            AssertMutable();
            _extraDamage = value;
        }
    }

    private void AddDamage(decimal amount)
    {
        if (amount <= 0m)
        {
            return;
        }
        DynamicVars.Damage.BaseValue += amount;
        ExtraDamage += amount;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamage;
    }
}