// using MegaCrit.Sts2.Core.CardSelection;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Relics;
// using MegaCrit.Sts2.Core.HoverTips;
// using MegaCrit.Sts2.Core.Models;
// using SnakeInSpireExtend.Scripts.Cards;
// using SnakeInSpireExtend.Scripts.RelicPools;
// using STS2RitsuLib.Interop.AutoRegistration;
// using STS2RitsuLib.Scaffolding.Content;

// namespace SnakeInSpireExtend.Scripts.Relics;

// [RegisterRelic(typeof(SnakeRelicPool))]
// public class TestRelic : ModRelicTemplate
// {
//     public override RelicRarity Rarity => RelicRarity.None;

//     public override RelicAssetProfile AssetProfile => new(
//         // 小图标（原版85x85）
//         IconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
//         // 轮廓图标（原版85x85）
//         IconOutlinePath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
//         // 大图标（原版256x256）
//         BigIconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png"
//     );

//     public override async Task AfterObtained()
//     {
//         CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(SelectionScreenPrompt, 1)
//         {
//             Cancelable = false,
//             RequireManualConfirmation = true
//         };
//         List<CardTransformation> transformations = (await CardSelectCmd.FromDeckForTransformation(Owner, cardSelectorPrefs, 
//         (CardModel c) => new CardTransformation(c, CreatePurgeFromOriginal(c, forPreview: true))))
//         .Select((CardModel original) => new CardTransformation(original, CreatePurgeFromOriginal(original, forPreview: false))).ToList();
//         await CardCmd.Transform(transformations, Owner.PlayerRng.Transformations);
//         await RelicCmd.Remove(this);
//     }

//     protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Purge>();

//     private CardModel CreatePurgeFromOriginal(CardModel original, bool forPreview)
//     {
//         CardModel cardModel = forPreview ? ModelDb.Card<Purge>().ToMutable() : Owner.RunState.CreateCard<Purge>(Owner);
//         if (original.IsUpgraded && cardModel.IsUpgradable)
//         {
//             if (forPreview)
//             {
//                 cardModel.UpgradeInternal();
//             }
//             else
//             {
//                 CardCmd.Upgrade(cardModel);
//             }
//         }
//         if (original.Enchantment != null)
//         {
//             EnchantmentModel enchantmentModel = (EnchantmentModel)original.Enchantment.MutableClone();
//             if (enchantmentModel.CanEnchant(cardModel))
//             {
//                 if (forPreview)
//                 {
//                     cardModel.EnchantInternal(enchantmentModel, enchantmentModel.Amount);
//                     enchantmentModel.ModifyCard();
//                 }
//                 else
//                 {
//                     CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
//                 }
//             }
//         }
//         return cardModel;
//     }
// }