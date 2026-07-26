using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;

namespace SnakeInSpireExtend.Scripts.Cards;

public class ScrapShield() : SnakeCardTemplate(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HysteresisVar", 2m),
        new DynamicVar("HasteVar", 2m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = CombatState.CreateCard<DefendSnake>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(card);
        }
        Helper.Hysteresis(card, DynamicVars["HysteresisVar"].BaseValue);
        Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
        CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<DefendSnake>(IsUpgraded),
        ..Helper.HysteresisHoverTipIfNeeded(this),
        ..Helper.HasteHoverTipIfNeeded(this),
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];
}