using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;

namespace SnakeInSpireExtend.Scripts.Vfx;

public partial class NShootVfx : Node2D
{
    private Node2D _primaryVfx = null!;

    private Vector2 _creatureCenter;

    private VfxColor _vfxColor;

    private Tween? _tween;

    public static NShootVfx? Create(Creature? target, VfxColor vfxColor = VfxColor.Gold, bool center = false)
    {
        if (NCombatRoom.Instance == null)
        {
            return null;
        }
        NCreature? creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
        if (creatureNode == null)
        {
            return null;
        }
        Vector2 vfxSpawnPosition = creatureNode.VfxSpawnPosition;
        NShootVfx nShootVfx = PreloadManager.Cache.GetScene("res://SnakeInSpireExtend/scenes/shoot_vfx.tscn").Instantiate<NShootVfx>(PackedScene.GenEditState.Disabled);
        nShootVfx._vfxColor = vfxColor;
        Vector2 vector = center ? new Vector2(0f, 0f) : new Vector2(Rng.Chaotic.NextFloat(-50f, 50f), Rng.Chaotic.NextFloat(-50f, 50f));
        nShootVfx._creatureCenter = vfxSpawnPosition + vector;
        return nShootVfx;
    }

    public override void _Ready()
    {
        _primaryVfx = GetNode<Node2D>("Primary");
        _primaryVfx.GlobalPosition = GenerateSpawnPosition();
        SetColor();
        TaskHelper.RunSafely(Animate());
    }

    private static readonly Dictionary<VfxColor, Color> ColorMap = new()
    {
        { VfxColor.Green, new Color("00A52F") },
        { VfxColor.Blue,  new Color("007BDD") },
        { VfxColor.Purple, new Color("A803FF") },
        { VfxColor.White, new Color("808080") },
        { VfxColor.Cyan,  new Color("009599") },
        { VfxColor.Gold,  new Color("EBA800") },
        { VfxColor.Red,  new Color("FF0000") },
    };

    private void SetColor()
    {
        if (_vfxColor == VfxColor.Black) return;
        _primaryVfx.SelfModulate = ColorMap.GetValueOrDefault(_vfxColor, new Color("FF0000"));
    }

    private Vector2 GenerateSpawnPosition()
    {
        Vector2 vector = new Vector2(Rng.Chaotic.NextFloat(-12f, 12f), Rng.Chaotic.NextFloat(-32f, 32f));
        Vector2 vector2 = new Vector2(-300f, 0f);
        return _creatureCenter + vector + vector2;
    }

    public override void _ExitTree()
    {
        _tween?.Kill();
    }

    private async Task Animate()
    {
        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(_primaryVfx, "position", _creatureCenter, 0.2).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
        _tween.TweenProperty(this, "modulate:a", 0f, 0.3).SetDelay(0.2);
        await _tween.AwaitFinished(this);
        this.QueueFreeSafely();
    }
}