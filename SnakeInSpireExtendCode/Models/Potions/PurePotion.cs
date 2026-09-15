using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace SnakeInSpireExtend.Scripts.Potions;

public class PurePotion : SnakePotionTemplate
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Adroit", 2m)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromEnchantment<Adroit>(DynamicVars["Adroit"].IntValue);

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        Player? player = target.Player;
        if(player == null) return;
        NCombatRoom.Instance?.PlaySplashVfx(target, Godot.Colors.White);
        foreach (CardModel card in PileType.Draw.GetPile(player).Cards.Where(c => ModelDb.Enchantment<Adroit>().CanEnchant(c)))
        {
            CardCmd.Enchant<Adroit>(card, DynamicVars["Adroit"].BaseValue);
        }
    }
}