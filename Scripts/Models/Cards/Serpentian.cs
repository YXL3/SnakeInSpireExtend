using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace SnakeInSpireExtend.Scripts.Cards;

public class Serpentian() : SnakeCardTemplate(3, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<IntangiblePower>(1m),
        new EnergyVar(1),
        new CardsVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(CombatState == null) return;
        // await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, DynamicVars["IntangiblePower"].BaseValue, Owner.Creature, this);
        IEnumerable<Player> players = CombatState.GetTeammatesOf(Owner.Creature).Where(c => c != null && c.IsAlive && c.Player != null && c != Owner.Creature).Select(c => c.Player!);
        foreach (Player player in players)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, player);
        }
        PlayerCmd.EndTurn(Owner, canBackOut: false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        EnergyHoverTip,
        HoverTipFactory.FromPower<IntangiblePower>()
    ];
}