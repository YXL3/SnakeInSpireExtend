using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Enchantment;

[RegisterEnchantment]
public class Cataclysm : ModEnchantmentTemplate
{
    public override bool HasExtraCardText => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Type")];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    public override EnchantmentAssetProfile AssetProfile => new(
        IconPath: $"res://SnakeInSpireExtend/images/enchantments/{GetType().Name}.png"
    );

    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Exhaust);
        ((StringVar)DynamicVars["Type"]).StringValue = CardTypeExtensions.ToLocString(Card.Type).GetRawText();
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (Status == EnchantmentStatus.Normal)
        {
            foreach(CardModel card in PileType.Hand.GetPile(Card.Owner).Cards.Where(card => card.Type == Card.Type).ToList())
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
    }
}