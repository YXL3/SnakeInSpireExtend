using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SnakeInSpireExtend.Scripts.Powers;

namespace SnakeInSpireExtend.Scripts.Cards;

public class KillerMove() : SnakeCardTemplate(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<KillerMovePower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay){
        await PowerCmd.Apply<KillerMovePower>(choiceContext, Owner.Creature, DynamicVars["KillerMovePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade(){
        EnergyCost.UpgradeBy(-1);
    }
}