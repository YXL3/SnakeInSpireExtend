using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using STS2RitsuLib.RunData;

namespace SnakeInSpireExtend.Scripts.Extension;

[RegisterSingleton]
public class SneakyPhantomSingleton() : HookedSingletonModel(HookType.Combat)
{
    private static readonly PlayerRunSavedData<PhantomCarryOverState> SavedData = Entry.SneakyPhantomSavedData;

    private PhantomCarryOverState? _pendingCarryOver;

    public static void StoreCarryOver(Player player, PhantomCarryOverState state)
    {
        if (ModelDb.Singleton<SneakyPhantomSingleton>() is not { } self) return;
        if (player.RunState is not RunState runState) return;
        self._pendingCarryOver = state;
        SavedData.Set(runState, player.NetId, state);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (CurrentCombatState?.RoundNumber != 1 || CurrentRunState is not RunState runState) return;
        PhantomCarryOverState? state = _pendingCarryOver;
        if (state == null || state.IsEmpty)
        {
            state = SavedData.Get(runState, player.NetId);
            if (state == null || state.IsEmpty)return;
        }
        await CreatureCmd.GainBlock(player.Creature, state.Block, ValueProp.Unpowered, null);
        await PlayerCmd.GainEnergy(state.Energy, player);
        foreach (PhantomCardEntry entry in state.Cards)
        {
            CardModel card = CardModel.FromSerializable(entry.Card);
            CurrentCombatState.AddCard(card, player);
            if (entry.ReplayCount > 0) card.BaseReplayCount = entry.ReplayCount;
            if (entry.Haste > 0) Helper.Haste(card, entry.Haste);
            if (entry.Hysteresis > 0) Helper.Hysteresis(card, entry.Hysteresis);
            PhantomEnergyCostCodec.Apply(card, entry.EnergyCost);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        }
        _pendingCarryOver = null;
        SavedData.Set(runState, player.NetId, new PhantomCarryOverState());
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is EventRoom eventRoom && eventRoom.CanonicalEvent is Neow && CurrentRunState is RunState runState)
        {
            _pendingCarryOver = null;
            foreach (Player player in runState.Players)
            {
                SavedData.Set(runState, player.NetId, new PhantomCarryOverState());
            }
        }
        return Task.CompletedTask;
    }
}
