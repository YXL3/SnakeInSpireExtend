// using System.Reflection;
// using MegaCrit.Sts2.Core.Entities.Relics;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Rooms;
// using SnakeInSpireExtend.Scripts.RelicPools;
// using STS2RitsuLib.Cards.DynamicVars;
// using STS2RitsuLib.Interop.AutoRegistration;
// using STS2RitsuLib.Scaffolding.Content;

// namespace SnakeInSpireExtend.Scripts.Relics;

// [RegisterRelic(typeof(SnakeRelicPool))]
// public class MarkA : ModRelicTemplate
// {
//     public override RelicRarity Rarity => RelicRarity.Starter;

//     private static readonly FieldInfo s_roomsField =
//         typeof(ActModel).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance)!;

//     protected override IEnumerable<DynamicVar> CanonicalVars => [
//         ModCardVars.String("NextMonster", "QwQ"),
//         ModCardVars.String("NextElite", "TAT")
//     ];

//     public override RelicAssetProfile AssetProfile => new(
//         IconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
//         IconOutlinePath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png",
//         BigIconPath: $"res://SnakeInSpireExtend/images/relics/{GetType().Name}.png"
//     );

//     // public override async Task AfterActEntered()
//     // {
//     //     ((StringVar)DynamicVars["NextMonster"]).StringValue = Owner.RunState.Act.PullNextEncounter(RoomType.Monster).Title.GetRawText();
//     //     ((StringVar)DynamicVars["NextElite"]).StringValue = Owner.RunState.Act.PullNextEncounter(RoomType.Elite).Title.GetRawText();
//     // }

//     public override async Task AfterRoomEntered(AbstractRoom room)
//     {
//         if (room.RoomType == RoomType.Monster)
//         {
//             RoomSet _rooms = (RoomSet)s_roomsField.GetValue(Owner.RunState.Act)!;
//             ((StringVar)DynamicVars["NextMonster"]).StringValue = 
//             _rooms.normalEncounters[(_rooms.normalEncountersVisited + 1) % _rooms.normalEncounters.Count].Title.GetRawText();
//         }
//         else
//         {
//             ((StringVar)DynamicVars["NextMonster"]).StringValue = Owner.RunState.Act.PullNextEncounter(RoomType.Monster).Title.GetRawText();
//         }
//         if (room.RoomType == RoomType.Elite)
//         {
//             RoomSet _rooms = (RoomSet)s_roomsField.GetValue(Owner.RunState.Act)!;
//             ((StringVar)DynamicVars["NextElite"]).StringValue = 
//             _rooms.eliteEncounters[(_rooms.eliteEncountersVisited + 1) % _rooms.eliteEncounters.Count].Title.GetRawText();
//         }
//         else
//         {
//             ((StringVar)DynamicVars["NextElite"]).StringValue = Owner.RunState.Act.PullNextEncounter(RoomType.Elite).Title.GetRawText();
//         }
//     }
// }
