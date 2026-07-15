using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

using STS2RitsuLib.Combat.Rewards;


namespace SnakeInSpireExtend.Scripts.Rewards;
public static class SnakeModRewardRegister
{
    /// <summary>
    ///     The deterministic RewardType that was registered for CardTransformReward.
    ///     Use this directly in CardTransformReward.ModRewardType so the serialized type
    ///     always matches the deserialization factory lookup.
    /// </summary>
    public static RewardType CardTransformRewardType { get; private set; }

    public static void TransformRegister()
    {
        var definition = ModRewardRegistry.For("SnakeInSpireExtend")
            .RegisterOwned("card_transform", (SerializableReward save, Player player, string? json) =>
            {
                CardModel? targetCard = null;
                if (save.SpecialCard != null)
                {
                    targetCard = CardModel.FromSerializable(save.SpecialCard);
                }

                if (targetCard != null)
                {
                    return new CardTransformReward(player, targetCard);
                }
                return new CardTransformReward(player);
            });
        CardTransformRewardType = definition.RewardType;
    }
}