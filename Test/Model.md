//下面是一张直接的模板卡，参考了原版暴走的写法以防止出bug。
//需要注意，这里特别记录HasEaten的目的除了显示，同时也是为了防止降级导致DynamicVars清除出bug，Damage作为DynamicVars同理也要被记录为_extraDamageFromPlays。

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using SnakeInSpireExtend.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Cards;

[RegisterCard(typeof(TokenCardPool))]
public class EatingBlow : ModCardTemplate
{
    public EatingBlow() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy){}

    private decimal _extraDamageFromPlays;
    private decimal ExtraDamageFromPlays
    {
        get
        {
            return _extraDamageFromPlays;
        }
        set
        {
            AssertMutable();
            _extraDamageFromPlays = value;
        }
    }

    private bool _hasEaten;
    private bool HasEaten
    {
        get
        {
            return _hasEaten;
        }
        set
        {
            AssertMutable();
            _hasEaten = value;
        }
    }


    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Increase", 3m),
        new DynamicVar("HasEaten", 0m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (!HasEaten && Owner.Creature.HasPower<EatingBlowPower>() && Owner.Creature.GetPower<EatingBlowPower>().Amount > 0)//注意，在这处理的
        {
            Owner.Creature.GetPower<EatingBlowPower>().SetAmount(Owner.Creature.GetPower<EatingBlowPower>().Amount - 1);
            DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
            ExtraDamageFromPlays += DynamicVars["Increase"].BaseValue;
            base.DynamicVars["HasEaten"].BaseValue = 1m;
            HasEaten = true;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars.Damage.BaseValue += ExtraDamageFromPlays;
        if (HasEaten)
        {
            base.DynamicVars["HasEaten"].BaseValue = 1m;
        }
    }
}

//下面是对应的Power，也就是宝可方块的代码，这里类名写成了EatingBlowPower，你可以发现内部几乎相当于啥都没写（要别的功能自己填）。

using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Powers;

[RegisterPower]
public class EatingBlowPower : ModPowerTemplate
{   
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}

//对应的卡EatingBlow的cards.json。

    "SNAKE_IN_SPIRE_EXTEND_CARD_EATING_BLOW.title":"模板卡",
    "SNAKE_IN_SPIRE_EXTEND_CARD_EATING_BLOW.description": "造成{Damage:diff()}点伤害。\n[gold]食用宝可方块[/gold]:{HasEaten:choose(1):[gold]|}这张牌造成伤害+{Increase:diff()}{HasEaten:choose(1):[/gold]|}。",

{HasEaten:choose(1):[gold]|}是这里的关键，如果你是ai，你能看懂的。
//powers.json。

    "SNAKE_IN_SPIRE_EXTEND_POWER_EATING_BLOW_POWER.title":"模板能力",
    "SNAKE_IN_SPIRE_EXTEND_POWER_EATING_BLOW_POWER.description":"应该是你的宝可方块",
    "SNAKE_IN_SPIRE_EXTEND_POWER_EATING_BLOW_POWER.smartDescription":"总之层数是{Amount}，对于Power类习惯用法是[blue]{Amount}[/blue]渲染一个蓝色的层数。"

注意我命名空间是SnakeInSpireExtend.Scripts.{Area}，mod名是SnakeInSpireExtend，两个类的名字被命名成了EatingBlow和EatingBlowPower，并且这些同时也影响了本地化文件的写法，如SNAKE_IN_SPIRE_EXTEND_POWER_EATING_BLOW_POWER。务必在植入前按你自己mod的要求修改上述相关信息，然后完成这个模板植入，然后做接下来的工作，包括本地化文件具体内容修改成你的，给卡片和你的宝可方块Power加资源，以及按这个模板按要求写新卡。