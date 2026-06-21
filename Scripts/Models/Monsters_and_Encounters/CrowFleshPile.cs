using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Monsters;

[RegisterMonster]
public class CrowFleshPile : ModMonsterTemplate
{
    public override MonsterAssetProfile AssetProfile => new(
        VisualsScenePath: "res://SnakeInSpireExtend/scenes/crow_flesh_pile.tscn"
    );
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 320, 300);

    public override int MaxInitialHp => MinInitialHp;

    private int ReigniteDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 10);

    private int ScratchWeaknessDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 25, 25);

    private int ScratchFrailDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 25, 25);

    private int BeakDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);

    private int SludgeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 15, 15);

    private int ReigniteReviveAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 45, 40);

    public override bool ShouldDisappearFromDoom => !this.Creature.HasPower<RevivePower>();
    
    private MoveState DeadState;

    private int _respawns;

    private int Respawns
    {
        get
        {
            return _respawns;
        }
        set
        {
            AssertMutable();
            _respawns = value;
        }
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        MoveState reignite = new MoveState("REIGNITE_MOVE", ReigniteMove, new MultiAttackIntent(ReigniteDamage, 2), new BuffIntent());
        MoveState scratchWeakness = new MoveState("SCRATCH_WEAKNESS_MOVE", ScratchWeaknessMove, new SingleAttackIntent(ScratchWeaknessDamage), new DebuffIntent());
        MoveState scratchFrailty = new MoveState("SCRATCH_FRAILTY_MOVE", ScratchFrailtyMove, new SingleAttackIntent(ScratchFrailDamage), new DebuffIntent());
        MoveState beak = new MoveState("BEAK_MOVE", BeakMove, new MultiAttackIntent(BeakDamage, 3));
        MoveState sludge = new MoveState("SLUDGE_MOVE", SludgeMove, new SingleAttackIntent(SludgeDamage), new StatusIntent(2));
        DeadState = new MoveState("RESPAWN_MOVE", RespawnMove, new HealIntent(), new BuffIntent())
        {
            MustPerformOnceBeforeTransitioning = true,
            FollowUpState = reignite
        };
        reignite.FollowUpState = scratchWeakness;
        scratchWeakness.FollowUpState = scratchFrailty;
        scratchFrailty.FollowUpState = beak;
        beak.FollowUpState = sludge;
        sludge.FollowUpState = scratchWeakness;
        return new MonsterMoveStateMachine([reignite, scratchWeakness, scratchFrailty, beak, sludge, DeadState], reignite);
    }

    private async Task ReigniteMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ReigniteDamage).WithHitCount(2).FromMonster(this).WithHitFx("vfx/vfx_bloody_impact").Execute(null);
        await PowerCmd.Apply<RevivePower>(new ThrowingPlayerChoiceContext(), base.Creature, Math.Max(1, ReigniteReviveAmount - Respawns * 5), base.Creature, null);
    }

    private async Task ScratchFrailtyMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ScratchFrailDamage).FromMonster(this).WithHitFx("vfx/vfx_scratch").Execute(null);
        await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), targets, 2m, base.Creature, null);
        await PowerCmd.Apply<RevivePower>(new ThrowingPlayerChoiceContext(), base.Creature, 15m, base.Creature, null);
    }

    private async Task ScratchWeaknessMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ScratchWeaknessDamage).FromMonster(this).WithHitFx("vfx/vfx_scratch").Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, 2m, base.Creature, null);
        await PowerCmd.Apply<RevivePower>(new ThrowingPlayerChoiceContext(), base.Creature, 15m, base.Creature, null);
    }

    private async Task BeakMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(BeakDamage).WithHitCount(3).FromMonster(this).WithHitFx("vfx/vfx_attack_blunt").Execute(null);
        await PowerCmd.Apply<RevivePower>(new ThrowingPlayerChoiceContext(), base.Creature, 15m, base.Creature, null);
    }

    private async Task SludgeMove(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(ScratchFrailDamage).FromMonster(this).WithHitFx("vfx/vfx_gaze").Execute(null);
        foreach (Creature target2 in targets)
        {
            Player player = target2.Player ?? target2.PetOwner;
            List<CardPileAddResult> statusCards = new List<CardPileAddResult>();
            for (int i = 0; i < 2; i++)
            {
                CardModel card = base.CombatState.CreateCard<MegaCrit.Sts2.Core.Models.Cards.Void>(player);
                PileType newPileType = (i == 0) ? PileType.Draw : PileType.Discard;
                List<CardPileAddResult> list = statusCards;
                list.Add(await CardPileCmd.AddGeneratedCardToCombat(card, newPileType, null, CardPilePosition.Random));
            }

            if (LocalContext.IsMe(player))
            {
                CardCmd.PreviewCardPileAdd(statusCards);
                await Cmd.Wait(1f);
            }
        }
        await PowerCmd.Apply<RevivePower>(new ThrowingPlayerChoiceContext(), base.Creature, 15m, base.Creature, null);
    }

    private async Task RespawnMove(IReadOnlyList<Creature> targets)
    {
        //await CreatureCmd.TriggerAnim(base.Creature, "RespawnTrigger", 0f);
        if (base.Creature.CombatState == null)
        {
            return;
        }
        AssertMutable();
        int reviveHp = base.Creature.GetPower<RevivePower>().Amount * base.Creature.CombatState.Players.Count;
        await CreatureCmd.SetMaxHp(base.Creature, reviveHp);
        await CreatureCmd.Heal(base.Creature, reviveHp);
        base.Creature.GetPower<RevivePower>()?.DoRevive();
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.Creature, base.Creature.GetPower<RevivePower>().ReviveStrength, base.Creature, null);
        await PowerCmd.Remove<RevivePower>(base.Creature);
        Respawns++;
    }

    public async Task TriggerDeadState()
    {
        //await CreatureCmd.TriggerAnim(base.Creature, "DeadTrigger", 0f);
        SetMoveImmediate(DeadState, forceTransition: true);
    }

    public override bool ShouldPowerBeRemovedOnDeath(PowerModel power)
    {
        if(power.GetType() == typeof(StrengthPower))
        {
            return false;
        }
        return base.ShouldPowerBeRemovedOnDeath(power);
    }
}