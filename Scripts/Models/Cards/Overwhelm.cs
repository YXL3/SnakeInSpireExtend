using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Models;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Overwhelm() : SnakeCardTemplate(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5m, ValueProp.Move),
        new RepeatVar(3),
        new DynamicVar("HasteVar", 2m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SnakeInSpireExtendCardKeywords.Keen];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitVfxNode(t => NStabVfx.Create(t, true, VfxColor.Gold))
            .WithHitFx(null, null, "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw){
        if (card == this)
        {
            Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(1m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [..Helper.HasteHoverTipIfNeeded(this)];
}