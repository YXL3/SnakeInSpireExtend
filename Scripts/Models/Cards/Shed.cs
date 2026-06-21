using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class Shed : ModCardTemplate
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HysteresisVar", 2m),
        new DynamicVar("HasteVar", 2m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    public Shed(): base(0, CardType.Skill, CardRarity.Common, TargetType.Self){}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = base.CombatState.CreateCard<DefendSnake>(base.Owner);
        if (base.IsUpgraded)
        {
            CardCmd.Upgrade(card);
        }
        Helper.Hysteresis(card, base.DynamicVars["HysteresisVar"].BaseValue);
        Helper.Haste(card, base.DynamicVars["HasteVar"].BaseValue);
        CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, base.Owner);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<DefendSnake>(base.IsUpgraded),
        Helper.HysteresisHoverTip(),
        Helper.HasteHoverTip(this)
    ];
}