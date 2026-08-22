using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class TailToHand() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("HasteVar", 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        foreach(CardModel card in PileType.Hand.GetPile(Owner).Cards.Where(card => card.Type == CardType.Attack))
        {
            Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
        }
    }

    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}


