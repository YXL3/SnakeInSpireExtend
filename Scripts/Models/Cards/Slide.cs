using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.Extension;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(SnakeCardPool))]
[RegisterCharacterStarterCard(typeof(Snake),1,Order = 3)]
public class Slide() : ModCardTemplate(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    // public override CardAssetProfile AssetProfile => new(
    //     PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
    // );

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar("HasteVar", 1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        foreach (CardModel item in await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this))
        {
            Helper.Haste(item, DynamicVars["HasteVar"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HasteVar"].UpgradeValueBy(1m);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        Helper.HasteHoverTip(this)
    ];
}//"SNAKE_IN_SPIRE_EXTEND_CARD_SLIDE.description":"抽1张牌。\n如果抽到的是攻击牌，敌人本回合受到的攻击伤害翻倍。\n如果抽到的是技能牌，给予2层[gold]虚弱[/gold]和[gold]易伤[/gold]。\n如果抽到的是能力牌，敌人失去2点[gold]力量[/gold]。\n如果都不是，给予1层[gold]死[/gold]。",