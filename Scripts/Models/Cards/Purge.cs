using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Rewards;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Purge() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(11m, ValueProp.Move),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(CombatState == null) return;
        AbstractRoom? currentRoom = CombatState.RunState.CurrentRoom;
        if (currentRoom is CombatRoom combatRoom)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
            AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitVfxNode(t => NStabVfx.Create(t, true, VfxColor.Gold))
                .WithHitFx(null, null, "blunt_attack.mp3")
                .Execute(choiceContext);
            if (shouldTriggerFatal && attackCommand.Results.SelectMany((List<DamageResult> r) => r).Any((DamageResult r) => r.WasTargetKilled))
            {
                CardModel cardModel = CombatState.CreateCard<Purge>(Owner);
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(cardModel);
                }
                combatRoom.AddExtraReward(Owner, new CardTransformReward(Owner, cardModel));
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];
}