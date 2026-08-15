using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.Powers;

namespace SnakeInSpireExtend.Scripts.Cards;

public class SneakyPhantom() : SnakeCardTemplate(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SneakyPhantomPower? power = await PowerCmd.Apply<SneakyPhantomPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if(power == null || Owner.PlayerCombatState == null)
        {
            return;
        }
        List<CardModel> handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
        power.SetProperty(handCards, Owner.Creature.Block, Owner.PlayerCombatState.Energy);

        PhantomCarryOverState carryOver = new PhantomCarryOverState
        {
            Cards = handCards.Select(c => new PhantomCardEntry
            {
                Card = c.ToSerializable(),
                ReplayCount = c.BaseReplayCount,
                Haste = Helper.HasCustomDynamic(c, "Haste") ? c.DynamicVars["Haste"].BaseValue : 0,
                Hysteresis = Helper.HasCustomDynamic(c, "Hysteresis") ? c.DynamicVars["Hysteresis"].BaseValue : 0,
                EnergyCost = PhantomEnergyCostCodec.Capture(c)
            }).ToList(),
            Block = Owner.Creature.Block,
            Energy = Owner.PlayerCombatState.Energy
        };
        SneakyPhantomSingleton.StoreCarryOver(Owner, carryOver);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
        EnergyHoverTip
    ];
}