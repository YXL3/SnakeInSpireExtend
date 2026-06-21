using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(TokenCardPool))]
public class Plague : ModCardTemplate
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );    
    private decimal _extraDamageFromPlaguePlays;

    private decimal ExtraDamageFromPlaguePlays
    {
        get
        {
            return _extraDamageFromPlaguePlays;
        }
        set
        {
            AssertMutable();
            _extraDamageFromPlaguePlays = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("Increase", 2m)
    ];

    public Plague() : base(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies){}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        IEnumerable<Plague> enumerable = base.Owner.PlayerCombatState.AllCards.OfType<Plague>();
        foreach (Plague item in enumerable)
        {
            item.BuffFromPlaguePlay(base.DynamicVars["Increase"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Increase"].UpgradeValueBy(1m);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars.Damage.BaseValue += ExtraDamageFromPlaguePlays;
    }

    private void BuffFromPlaguePlay(decimal extraDamage)
    {
        base.DynamicVars.Damage.BaseValue += extraDamage;
        ExtraDamageFromPlaguePlays += extraDamage;
    }
}