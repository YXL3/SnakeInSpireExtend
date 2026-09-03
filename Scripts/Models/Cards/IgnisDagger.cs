using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace SnakeInSpireExtend.Scripts.Cards;

public class IgnisDagger() : SnakeCardTemplate(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(20m, ValueProp.Move)
    ];

    private Player? chosenPlayer;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(CombatState == null) return;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (chosenPlayer != null)
        {
            CardModel card = CombatState.CreateCard<Burn>(chosenPlayer);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }

    protected override CardLocation GetResultLocationForCardPlay()
    {
        CardLocation resultLocationForCardPlay = base.GetResultLocationForCardPlay();
        if (CombatState == null)
        {
            return resultLocationForCardPlay;
        }
        IEnumerable<Creature> list = CombatState.GetTeammatesOf(Owner.Creature).Where(c => c != null && c.IsAlive && c.IsPlayer && c.Player != Owner);
        chosenPlayer = Owner.RunState.Rng.CombatTargets.NextItem(list)?.Player;
        if (chosenPlayer == null)
        {
            return resultLocationForCardPlay;
        }
        if (resultLocationForCardPlay.pileType == PileType.Discard)
        {
            resultLocationForCardPlay.player = chosenPlayer;
            resultLocationForCardPlay.pileType = PileType.Hand;
            resultLocationForCardPlay.position = CardPilePosition.Bottom;
        }
        return resultLocationForCardPlay;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromCard<Burn>()
    ];
}