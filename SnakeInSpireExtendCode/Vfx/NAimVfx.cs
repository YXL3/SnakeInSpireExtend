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

public partial class NAimVfx : Node2D
{
    private Node2D _primaryVfx = null!;

    private Vector2 _creatureCenter;

    private VfxColor _vfxColor;

    private Tween? _tween;

    public static NAimVfx? Create(Creature? target, VfxColor vfxColor = VfxColor.Red)
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
        NAimVfx nAimVfx = PreloadManager.Cache.GetScene("res://SnakeInSpireExtend/scenes/aim_vfx.tscn").Instantiate<NAimVfx>(PackedScene.GenEditState.Disabled);
        nAimVfx._vfxColor = vfxColor;
        nAimVfx._creatureCenter = vfxSpawnPosition;
        return nAimVfx;
    }

    public override void _Ready()
    {
        _primaryVfx = GetNode<Node2D>("Primary");
        _primaryVfx.GlobalPosition = GenerateSpawnPosition();
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0f);
        TaskHelper.RunSafely(Animate());
    }

    private Vector2 GenerateSpawnPosition()
    {
        Vector2 vector = new Vector2(Rng.Chaotic.NextFloat(-200f, 200f), Rng.Chaotic.NextFloat(-200f, 200f));
        return _creatureCenter + vector;
    }

    public override void _ExitTree()
    {
        _tween?.Kill();
    }

    private async Task Animate()
    {
        _tween = CreateTween().SetParallel();
        _tween.TweenProperty(this, "modulate:a", 0.8f, 0.75).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        _tween.TweenProperty(_primaryVfx, "position", _creatureCenter, 0.75).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        _tween.TweenProperty(_primaryVfx, "rotation", _primaryVfx.Rotation + Mathf.Pi/4, 0.25).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetDelay(0.75);
        _tween.TweenProperty(this, "modulate:a", 0f, 0.25).SetDelay(0.75);
        await _tween.AwaitFinished(this);
        this.QueueFreeSafely();
    }
}