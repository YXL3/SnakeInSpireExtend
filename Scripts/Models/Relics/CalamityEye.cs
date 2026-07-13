using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SnakeInSpireExtend.Scripts.Extension;
using SnakeInSpireExtend.Scripts.RelicPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Relics;

[RegisterRelic(typeof(SnakeRelicPool))]
public class CalamityEye : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicAssetProfile AssetProfile => new(
        // 小图标（原版85x85）
        IconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
        // 轮廓图标（原版85x85）
        IconOutlinePath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
        // 大图标（原版256x256）
        BigIconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png"
    );
    private bool _wasUsedThisTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HysteresisVar", 1m),
        new DynamicVar("HasteVar", 1m)
    ];

    private bool WasUsedThisTurn
    {
        get
        {
            return _wasUsedThisTurn;
        }
        set
        {
            AssertMutable();
            _wasUsedThisTurn = value;
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (CombatManager.Instance.IsInProgress && !fromHandDraw && card.Owner == Owner && card.Owner.Creature.CombatState.CurrentSide == card.Owner.Creature.Side && !WasUsedThisTurn)
        {
            Flash();
            WasUsedThisTurn = true;
            Helper.Hysteresis(card, DynamicVars["HysteresisVar"].BaseValue);
            Helper.Haste(card, DynamicVars["HasteVar"].BaseValue);
        }
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
        {
            return Task.CompletedTask;
        }
        WasUsedThisTurn = false;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        WasUsedThisTurn = false;
        return Task.CompletedTask;
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        Helper.HysteresisHoverTip(),
        Helper.HasteHoverTip(this)
    ];
}