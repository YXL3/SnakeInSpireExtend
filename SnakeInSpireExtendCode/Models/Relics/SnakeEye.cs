using MegaCrit.Sts2.Core.Entities.Relics;
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
[RegisterCharacterStarterRelic(typeof(Snake))]
public class SnakeEye : ModRelicTemplate, IHasteModifier
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HasteDrawingAmount", 1m)
    ];

    private bool _usedThisCombat;

    public bool UsedThisCombat
    {
        get
        {
            return _usedThisCombat;
        }
        private set
        {
            if (_usedThisCombat != value)
            {
                AssertMutable();
                _usedThisCombat = value;
            }
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        UsedThisCombat = false;
        return Task.CompletedTask;
    }


    public decimal ReadHasteModifier(CardModel card, decimal currentValue)
    {
        if (!UsedThisCombat)
        {
            return DynamicVars["HasteDrawingAmount"].BaseValue;
        }
        else
        {
            return 0m;
        }
    }

    public async Task<decimal> ApplyHasteModifier(CardModel card, decimal currentValue)
    {
        if (!UsedThisCombat)
        {
            Flash();
            UsedThisCombat = true;
            return DynamicVars["HasteDrawingAmount"].BaseValue;
        }
        else
        {
            return 0m;
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        Helper.HasteHoverTip(this)
    ];
}