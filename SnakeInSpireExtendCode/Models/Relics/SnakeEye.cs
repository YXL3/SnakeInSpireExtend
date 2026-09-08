using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;

namespace SnakeInSpireExtend.Scripts.Relics;

[RegisterCharacterStarterRelic(typeof(Snake))]
public class SnakeEye : SnakeRelicTemplate, IHasteModifier
{
    public override RelicRarity Rarity => RelicRarity.Starter;

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

    public override Task BeforeCombatStart()
    {
        UsedThisCombat = false;
        Status = RelicStatus.Active;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
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
            Status = RelicStatus.Normal;
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