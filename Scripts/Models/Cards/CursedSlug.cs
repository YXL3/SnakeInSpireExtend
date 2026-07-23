using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class CursedSlug() : ModCardTemplate(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9m, ValueProp.Move),
        new RepeatVar(3),
        new CardsVar(3),
        new DynamicVar("HasteVar", 1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .WithHitFx("vfx/vfx_attack_blunt")
            .OnlyPlayAnimOnce()
            .Execute(choiceContext);
        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            CardModel card = CombatState.CreateCard<Slimed>(Owner);
            Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw){
        if (card == this)
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }
    }

    protected override void OnUpgrade(){
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<Slimed>(),
        ..Helper.HasteHoverTipIfNeeded(this)
    ];
}