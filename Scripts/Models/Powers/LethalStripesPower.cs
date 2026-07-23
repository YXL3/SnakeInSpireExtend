using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class LethalStripesPower : ModPowerTemplate
{
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
        BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
    );
    
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    private AttackCommand? record;

    public override async Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker == Owner && command.DamageProps.IsPoweredAttack() && command.ModelSource is CardModel
        && Traverse.Create(command).Field("_hitCount").GetValue<int>() > 1)
        {
            record = command;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (record != null && cardPlay.Card == record.ModelSource)
        {
            await CardCmd.Exhaust(choiceContext, cardPlay.Card);
            await PowerCmd.Apply<VigorPower>(choiceContext, Owner,
            record.Results.SelectMany((List<DamageResult> r) => r).Sum((DamageResult r) => r.TotalDamage), Owner, null);
            record = null;
        }
    }
}
