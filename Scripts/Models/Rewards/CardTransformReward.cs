using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Combat.Rewards;


namespace SnakeInSpireExtend.Scripts.Rewards;

public class CardTransformReward : ModCustomReward
{
    private CardModel? _targetCard;

    private bool hasTarget;

    public CardTransformReward(Player player, CardModel target) : base(player) 
    { 
        _targetCard = target;
        hasTarget = true;
    }

    public CardTransformReward(Player player) : base(player) 
    {
        _targetCard = null;
        hasTarget = false;
    }

    public override void MarkContentAsSeen()
    {
    }

    /// <summary>
    ///     Uses the RewardType captured during registration, ensuring the serialized value
    ///     always matches the factory lookup key in RegistrationsByType.
    /// </summary>
    public override RewardType ModRewardType => SnakeModRewardRegister.CardTransformRewardType;

    protected override string? RewardIconPath => "res://SnakeInSpireExtend/images/rewards/transform.png";

    public override LocString Description
    {
        get
        {
            LocString locString;
            if (hasTarget && _targetCard != null)
            {
                locString = new LocString("gameplay_ui", "COMBAT_REWARD_TRANSFORM_CARD_TO_CERTAIN");
                locString.Add("Card", _targetCard.Title);
            }
            else
            {
                locString = new LocString("gameplay_ui", "COMBAT_REWARD_TRANSFORM_CARD");
            }
            return locString;
        }
    }

    protected override async Task<bool> OnSelect()
    {
        CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(new LocString("gameplay_ui", "COMBAT_REWARD_TRANSFORM_CARD.selectionScreenPrompt"), 1)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };
        CardModel card = (await CardSelectCmd.FromDeckForTransformation(base.Player, cardSelectorPrefs)).FirstOrDefault();
        if (card != null)
        {
            if (hasTarget && _targetCard != null)
            {
                // Log.Info($"Original Owner: {(card.Owner != null ? $"{card.Owner.NetId} (ID: {card.Owner.GetHashCode()})" : "null")}");
                // Log.Info($"Replacement Owner: {(_targetCard.Owner != null ? $"{_targetCard.Owner.NetId} (ID: {_targetCard.Owner.GetHashCode()})" : "null")}");
                // Log.Info($"Are references equal? {object.ReferenceEquals(card.Owner, _targetCard.Owner)}");
                if(_targetCard.Owner == null)
                {
                    _targetCard.Owner = card.Owner;
                }
                await CardCmd.Transform(card, _targetCard);
                Log.Info($"Player {base.Player.NetId} transformed {card.Id} to {_targetCard.Id} from deck");
            }
            else
            {
                await CardCmd.TransformToRandom(card, base.Player.RunState.Rng.Niche);
                Log.Debug($"Player {base.Player.NetId} transformed {card.Id} to random from deck");
            }
            return true;
        }
        return false;
    }
    public override SerializableReward ToSerializable()
    {
        var result = ModRewardSerialization.CreateSerializable(this);
        if (_targetCard != null)
        {
            result.SpecialCard = _targetCard.ToSerializable();
        }
        return result;
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips {
        get
        {
            if(_targetCard != null)
            {
                return [HoverTipFactory.FromCard(_targetCard)];
            }
            else
            {
                return Array.Empty<IHoverTip>();
            }
        }
    }
}