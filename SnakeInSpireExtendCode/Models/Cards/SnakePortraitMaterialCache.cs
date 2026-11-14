using Godot;

namespace SnakeInSpireExtend.Scripts.Cards;

/// <summary>
/// Creates one shared nearest-neighbor portrait material per portrait texture path.
/// The shader samples its bound texture itself, so no CanvasItem.TextureFilter state
/// is written to pooled card nodes.
/// </summary>
internal static class SnakePortraitMaterialCache
{
    private const string ShaderPath = "res://SnakeInSpireExtend/shaders/card_portrait_nearest.gdshader";
    private const string TextureParameter = "portrait_texture";

    private static readonly Dictionary<string, ShaderMaterial> Materials = new(StringComparer.Ordinal);
    private static readonly HashSet<string> WarnedTexturePaths = new(StringComparer.Ordinal);

    private static Shader? _shader;
    private static bool _shaderLoadFailureLogged;

    public static ShaderMaterial? Get(string portraitPath)
    {
        if (Materials.TryGetValue(portraitPath, out ShaderMaterial? cached))
        {
            if (GodotObject.IsInstanceValid(cached))
            {
                return cached;
            }

            Materials.Remove(portraitPath);
        }

        Shader? shader = GetShader();
        if (shader == null)
        {
            return null;
        }

        Texture2D? texture = ResourceLoader.Load<Texture2D>(
            portraitPath,
            null,
            ResourceLoader.CacheMode.Reuse);

        if (texture == null)
        {
            if (WarnedTexturePaths.Add(portraitPath))
            {
                Entry.Logger.Warn($"[NearestPortrait] Failed to load portrait texture '{portraitPath}'.");
            }

            return null;
        }

        ShaderMaterial material = new()
        {
            Shader = shader
        };
        material.SetShaderParameter(TextureParameter, texture);
        Materials[portraitPath] = material;
        return material;
    }

    private static Shader? GetShader()
    {
        if (_shader != null && GodotObject.IsInstanceValid(_shader))
        {
            return _shader;
        }

        _shader = ResourceLoader.Load<Shader>(
            ShaderPath,
            null,
            ResourceLoader.CacheMode.Reuse);

        if (_shader != null && GodotObject.IsInstanceValid(_shader))
        {
            return _shader;
        }

        _shader = null;
        if (!_shaderLoadFailureLogged)
        {
            _shaderLoadFailureLogged = true;
            Entry.Logger.Warn($"[NearestPortrait] Failed to load shader '{ShaderPath}'.");
        }

        return null;
    }
}
