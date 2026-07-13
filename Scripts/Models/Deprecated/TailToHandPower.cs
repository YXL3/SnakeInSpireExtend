// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Players;
// using MegaCrit.Sts2.Core.Entities.Powers;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using MegaCrit.Sts2.Core.Models;
// using SnakeInSpireExtend.Scripts.Extension;
// using STS2RitsuLib.Interop.AutoRegistration;
// using STS2RitsuLib.Scaffolding.Content;

// namespace SnakeInSpireExtend.Scripts.Powers;

// [RegisterPower]
// public class TailToHandPower : ModPowerTemplate
// {
//     // public override PowerAssetProfile AssetProfile => new(
//     //     IconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png",
//     //     BigIconPath: $"res://SnakeInSpireExtend/images/powers/{GetType().Name}.png"
//     // );
    
//     private class Data
//     {
//         public CardModel? selectedCard;
//     }
    
//     public override PowerType Type => PowerType.Buff;

//     public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

//     public override PowerStackType StackType => PowerStackType.Counter;

//     protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Card")];

//     protected override object InitInternalData()
//     {
//         return new Data();
//     }

//     public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
//     {
//         if (player == Owner.Player)
//         {
//             CardModel? card = GetInternalData<Data>().selectedCard;
//             CardCmd.ClearAffliction(card);
//             Helper.Haste(card, Amount);
//             await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
//             await PowerCmd.Remove(this);
//         }
//     }

//     public void SetSelectedCard(CardModel card)
//     {
//         GetInternalData<Data>().selectedCard = card;
//         ((StringVar)DynamicVars["Card"]).StringValue = card.Title;
//     }
// }
