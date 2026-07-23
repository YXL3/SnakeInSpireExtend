using MegaCrit.Sts2.Core.CardSelection;
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
public class Psychokinesis() : ModCardTemplate(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HysteresisVar", 1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, this)).FirstOrDefault();
        if(card == null) return;
        await CardCmd.Discard(choiceContext, card);
        Helper.Hysteresis(card, DynamicVars["HysteresisVar"].BaseValue);
        card.SetToFreeThisCombat();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HysteresisVar"].UpgradeValueBy(1m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..Helper.HysteresisHoverTipIfNeeded(this)
    ];
}