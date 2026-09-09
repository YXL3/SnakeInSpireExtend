using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace SnakeInSpireExtend.Scripts.Potions;

public class PurePotion : SnakePotionTemplate
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Adroit", 1m)];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        Player? player = target.Player;
        if(player == null) return;
        foreach (CardModel card in PileType.Draw.GetPile(player).Cards.Where(c => ModelDb.Enchantment<Adroit>().CanEnchant(c)))
        {
            CardCmd.Enchant<Adroit>(card, DynamicVars["Adroit"].BaseValue);
        }
    }
}