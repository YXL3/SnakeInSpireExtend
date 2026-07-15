// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.HoverTips;
// using MegaCrit.Sts2.Core.Models.CardPools;
// using MegaCrit.Sts2.Core.Models.Cards;
// using STS2RitsuLib.Interop.AutoRegistration;
// using STS2RitsuLib.Scaffolding.Content;

// namespace SnakeInSpireExtend.Scripts.Cards;

// [RegisterCard(typeof(IroncladCardPool))]
// public class SymbiosisTouch() : ModCardTemplate(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
// {
//     public override CardAssetProfile AssetProfile => new(
//         PortraitPath: $"res://SnakeInSpireExtend/images/cards/{GetType().Name}.png"
//     );

//     public override IEnumerable<CardKeyword> CanonicalKeywords => [
//         CardKeyword.Ethereal
//     ];

//     public override TargetType TargetType => IsUpgraded? TargetType.AnyPlayer:TargetType.AnyAlly;

//     protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromAffliction<Corrupted>(1);
// }