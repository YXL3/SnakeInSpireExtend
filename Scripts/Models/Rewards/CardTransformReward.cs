using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Combat.Rewards;


namespace SnakeInSpireExtend.Scripts.Rewards;

public class CardTransformReward : ModCustomReward
{
    private CardModel? _targetCard;

    public CardTransformReward(Player player, CardModel target) : base(player) 
    { 
        _targetCard = target;
    }

    public CardTransformReward(Player player) : base(player) 
    {
        _targetCard = null;
    }

    public override void MarkContentAsSeen(){}

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
            if (_targetCard != null)
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
        if (_targetCard != null)
        {
            CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(new LocString("gameplay_ui", "COMBAT_REWARD_TRANSFORM_CARD_TO_CERTAIN.selectionScreenPrompt"), 1)
            {
                Cancelable = true,
                RequireManualConfirmation = true
            };
            if(_targetCard.Owner == null)
            {
                _targetCard.Owner = Player;
            }
            List<CardTransformation> transformations = (await CardSelectCmd.FromDeckForTransformation(Player, cardSelectorPrefs,
            (CardModel c) => new CardTransformation(c, _targetCard)))
            .Select((CardModel original) => new CardTransformation(original, _targetCard)).ToList();
            await CardCmd.Transform(transformations, Player.PlayerRng.Transformations);
        }
        else
        {
            CardModel? card = (await CardSelectCmd.FromDeckForTransformation(Player, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1))).FirstOrDefault();
            if (card == null)
            {
                return false;
            }
            await CardCmd.TransformToRandom(card, Player.PlayerRng.Transformations);
        }
        return true;
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