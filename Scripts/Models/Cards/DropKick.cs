using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Models;
namespace SnakeInSpireExtend.Scripts.Cards;

public class DropKick() : SnakeCardTemplate(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(15m, ValueProp.Move),
        new CardsVar(1),
        new DynamicVar("HasteVar", 2m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SnakeInSpireExtendCardKeywords.Keen];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
        foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue), null, this))
        {
            Helper.Haste(item, DynamicVars["HasteVar"].BaseValue);
        }
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [..Helper.HasteHoverTipIfNeeded(this)];
}
