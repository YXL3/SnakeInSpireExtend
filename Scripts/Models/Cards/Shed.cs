using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SnakeInSpireExtend.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
public class Shed() : ModCardTemplate(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );
    public override int MaxUpgradeLevel => int.MaxValue;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["StrengthPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }

    public override Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        if (player != Owner || Pile == null || Pile.Type != PileType.Deck)
        {
            return Task.CompletedTask;
        }
        CardCmd.Upgrade(this);
        return Task.CompletedTask;
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];
}