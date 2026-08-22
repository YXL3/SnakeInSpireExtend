using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

public class IronGem() : SnakeCardTemplate(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Adroit", 5m)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        Adroit canonicalEnchantment = ModelDb.Enchantment<Adroit>();
        foreach (CardModel card in await CardSelectCmd.FromHand(choiceContext, Owner, prefs,
         c => c.Type == CardType.Attack && ModelDb.Enchantment<Adroit>().CanEnchant(c), this))
        {
            CardCmd.Enchant<Adroit>(card, DynamicVars["Adroit"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Adroit"].UpgradeValueBy(2m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromEnchantment<Adroit>(DynamicVars["Adroit"].IntValue);
}