using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace SnakeInSpireExtend.Scripts.Relics;

public class BottledTide : SnakeRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(3)
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

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == Owner && PileType.Hand.GetPile(Owner).Cards.Count >= CardPile.MaxCardsInHand && !UsedThisCombat)
        {
            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
            UsedThisCombat = true;
            Status = RelicStatus.Normal;
        }
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.ForEnergy(this),
    ];
}