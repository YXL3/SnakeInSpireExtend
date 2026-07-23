using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SnakeInSpireExtend.Scripts.CardPools;
using SnakeInSpireExtend.Scripts.PotionPools;
using SnakeInSpireExtend.Scripts.RelicPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;

namespace SnakeInSpireExtend.Scripts;

[RegisterCharacter]
public class Snake : ModCharacterTemplate<SnakeCardPool, SnakeRelicPool, SnakePotionPool>
{
    // 角色名称颜色
    public override Color NameColor => new(0.5f, 1f, 0f);
    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new(0f, 0f, 0f);
    // 地图绘制颜色
    public override Color MapDrawingColor => new(0.5f, 1f, 0f);

    public override CharacterGender Gender => CharacterGender.Neutral;

    public override int StartingHp => 63;

    public override int StartingGold => 99;

    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Merge(
        CharacterAssetProfiles.Ironclad(),// 暂时先用 Ironclad 的全部配置，逐个取消注释替换为你自己的资源
        new(
            // Scenes: new(
            //     VisualsPath: "res://SnakeInSpireExtend/scenes/snake_visuals.tscn"
            // ),
            Ui: new(
                // 至少需要一个角色选择图标，否则选人界面会出问题
                CharacterSelectIconPath: "res://SnakeInSpireExtend/images/_material/snakeButton.png"
                // CharacterSelectBgPath: "res://SnakeInSpireExtend/scenes/snake_bg.tscn",
                // IconTexturePath: "res://SnakeInSpireExtend/images/snake/energy_snake.png"
            ),
            Scenes: new(
                EnergyCounterPath: "res://SnakeInSpireExtend/scenes/snake_energy_counter.tscn"
            )
            // Vfx: new(
            //     TrailPath: "..."
            // ),
            // Audio: new(
            //     CharacterSelectSfx: "..."
            // ),
            // Multiplayer: new(
            // )
        ));

    // 攻击和施法动画延迟，以对齐动画
    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    // 如果你的人物不需要时间线小故事，加上这句。
    public override bool RequiresEpochAndTimeline => false;

    // 自动转换人物场景，让你不需要手动挂脚本。复制即可。
    protected override NCreatureVisuals? TryCreateCreatureVisuals() => RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>(AssetProfile.Scenes!.VisualsPath!);

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_bite",
        "vfx/vfx_attack_slash",
        "vfx/vfx_thrash",
        "vfx/vfx_grand_finale_impact"
    ];
}