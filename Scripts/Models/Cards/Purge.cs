using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Models;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Purge() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("HasteDrawingAmount", 2m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        SnakeInSpireExtendCardKeywords.Keen
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [..Helper.HasteHoverTipIfNeeded(this)];
}