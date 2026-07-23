using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Models;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class FrozenLen() : ModCardTemplate(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<FrozenLenPower>(1m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        await PowerCmd.Apply<FrozenLenPower>(choiceContext, Owner.Creature, DynamicVars["FrozenLenPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromKeyword(SnakeInSpireExtendCardKeywords.Keen)];
}