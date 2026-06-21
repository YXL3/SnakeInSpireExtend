using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using SnakeInSpireExtend.Scripts.Monsters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace SnakeInSpireExtend.Scripts.Encounters;

[RegisterActEncounter(typeof(Glory))]
public class CrowFleshPileBoss : ModEncounterTemplate
{
    public override EncounterAssetProfile AssetProfile => new(
        RunHistoryIconPath: "res://SnakeInSpireExtend/images/ui/run_history/crow_flesh_pile_boss.png",
        RunHistoryIconOutlinePath: "res://SnakeInSpireExtend/images/ui/run_history/crow_flesh_pile_boss_outline.png"
    );
    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<CrowFleshPile>()];

    public override RoomType RoomType => RoomType.Boss;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() => [
        (ModelDb.Monster<CrowFleshPile>().ToMutable(), null)
    ];
}