using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using MegaCrit.Sts2.Core.Modding;

namespace SnakeInSpireExtend.Loader;

[ModInitializer(nameof(Initialize))]
public static class Bootstrap
{
    private const string VariantManifestName = "snake-in-spire-extend.variants.json";
    private const string CompatDirectoryName = "compat";
    private const string SharedDirectoryName = "shared";
    private const string VariantAssemblyName = "SnakeInSpireExtend";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;

        try
        {
            var loaderDirectory = Path.GetDirectoryName(typeof(Bootstrap).Assembly.Location);
            if (string.IsNullOrWhiteSpace(loaderDirectory))
            {
                LogError("Could not resolve the loader directory.");
                return;
            }

            var manifestPath = Path.Combine(loaderDirectory, VariantManifestName);
            if (!File.Exists(manifestPath))
            {
                LogError($"Missing variant manifest: {manifestPath}");
                return;
            }

            VariantManifest? manifest;
            try
            {
                manifest = JsonSerializer.Deserialize<VariantManifest>(
                    File.ReadAllText(manifestPath),
                    JsonOptions);
            }
            catch (Exception ex)
            {
                LogError($"Failed to read variant manifest: {ex}");
                return;
            }

            if (manifest?.Variants is not { Count: > 0 })
            {
                LogError($"Variant manifest contains no variants: {manifestPath}");
                return;
            }

            var hostVersion = HostVersionResolver.Resolve();
            var variant = SelectVariant(manifest.Variants, hostVersion);
            if (variant?.CompatTarget is null)
            {
                LogError("No compatible SnakeInSpireExtend variant was found.");
                return;
            }

            var loadContext = AssemblyLoadContext.GetLoadContext(typeof(Bootstrap).Assembly)
                               ?? AssemblyLoadContext.Default;

            var loadedAssemblies = new List<Assembly>();

            foreach (var file in manifest.Shared ?? new List<VariantFile>())
            {
                var sharedPath = ResolveAssemblyPath(
                    Path.Combine(loaderDirectory, SharedDirectoryName),
                    file);
                if (sharedPath is null)
                {
                    LogError($"Shared assembly not found: {file.Assembly}");
                    return;
                }

                loadedAssemblies.Add(loadContext.LoadFromAssemblyPath(sharedPath));
            }

            var variantDirectory = Path.Combine(
                loaderDirectory,
                CompatDirectoryName,
                variant.CompatTarget);

            foreach (var file in variant.Files ?? new List<VariantFile>())
            {
                var assemblyPath = ResolveAssemblyPath(variantDirectory, file);
                if (assemblyPath is null)
                {
                    LogError($"Variant assembly not found: {file.Assembly}");
                    return;
                }

                loadedAssemblies.Add(loadContext.LoadFromAssemblyPath(assemblyPath));
            }

            var mainAssembly = loadedAssemblies.LastOrDefault(
                assembly => string.Equals(
                    assembly.GetName().Name,
                    VariantAssemblyName,
                    StringComparison.OrdinalIgnoreCase));

            if (mainAssembly is null)
            {
                LogError($"Variant did not load assembly '{VariantAssemblyName}'.");
                return;
            }

            InvokeModInitializer(mainAssembly);
        }
        catch (Exception ex)
        {
            LogError(ex.ToString());
        }
    }

    private static VariantEntry? SelectVariant(
        IReadOnlyList<VariantEntry> variants,
        Version? hostVersion)
    {
        var parsed = variants
            .Select(entry => new
            {
                Entry = entry,
                Version = ParseCompatTarget(entry.CompatTarget),
            })
            .Where(item => item.Version is not null)
            .OrderBy(item => item.Version)
            .ToList();

        if (parsed.Count == 0)
            return null;

        if (hostVersion is null)
            return parsed[^1].Entry;

        var candidates = parsed
            .Where(item => item.Version <= hostVersion)
            .ToList();

        return candidates.Count > 0 ? candidates[^1].Entry : parsed[^1].Entry;
    }

    private static Version? ParseCompatTarget(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var value = text.Trim();

        var dash = value.IndexOf('-', StringComparison.Ordinal);
        if (dash >= 0)
            value = value[..dash].Trim();

        var plus = value.IndexOf('+', StringComparison.Ordinal);
        if (plus >= 0)
            value = value[..plus].Trim();

        if (value.Length >= 2 &&
            (value[0] == 'v' || value[0] == 'V') &&
            char.IsDigit(value[1]))
        {
            value = value[1..];
        }

        return Version.TryParse(value, out var version) ? version : null;
    }

    private static string? ResolveAssemblyPath(string directory, VariantFile file)
    {
        if (string.IsNullOrWhiteSpace(file.Assembly))
            return null;

        var candidate = Path.Combine(directory, file.Assembly.Trim());
        if (File.Exists(candidate))
            return candidate;

        if (!candidate.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            candidate += ".dll";
            if (File.Exists(candidate))
                return candidate;
        }

        return null;
    }

    private static void InvokeModInitializer(Assembly assembly)
    {
        Type?[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types;
        }

        foreach (var type in types)
        {
            if (type is null)
                continue;

            var attribute = type.GetCustomAttribute<ModInitializerAttribute>();
            if (attribute is null)
                continue;

            var methodName =
                attribute.GetType()
                    .GetField(
                        "initializerMethod",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(attribute) as string
                ?? attribute.GetType()
                    .GetProperty(
                        "initializerMethod",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(attribute) as string;

            if (string.IsNullOrWhiteSpace(methodName))
                continue;

            var method = type.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (method is null)
                continue;

            method.Invoke(null, null);
            return;
        }

        LogError($"No ModInitializer found in {assembly.FullName}.");
    }

    private static void LogError(string message)
    {
        Console.Error.WriteLine($"[SnakeInSpireExtend.Loader] {message}");
    }

    private static class HostVersionResolver
    {
        public static Version? Resolve()
        {
            foreach (var path in EnumerateReleaseInfoPaths())
            {
                if (TryReadVersion(path, out var version))
                    return version;
            }

            return null;
        }

        private static IEnumerable<string> EnumerateReleaseInfoPaths()
        {
            var processDirectory = Path.GetDirectoryName(Environment.ProcessPath ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(processDirectory))
                yield return Path.Combine(processDirectory, "release_info.json");

            var baseDirectory = AppContext.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(baseDirectory))
                yield return Path.Combine(baseDirectory, "release_info.json");

            var godotDataDirectory = TryCallGodotOsString("GetDataDir");
            if (!string.IsNullOrWhiteSpace(godotDataDirectory))
                yield return Path.Combine(godotDataDirectory, "game", "release_info.json");
        }

        private static bool TryReadVersion(string path, out Version? version)
        {
            version = null;

            try
            {
                if (!File.Exists(path))
                    return false;

                using var document = JsonDocument.Parse(File.ReadAllText(path));
                if (!document.RootElement.TryGetProperty("version", out var versionElement))
                    return false;

                return TryParseVersion(versionElement.GetString(), out version);
            }
            catch
            {
                return false;
            }
        }

        private static bool TryParseVersion(string? text, out Version? version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var value = text.Trim();

            var dash = value.IndexOf('-', StringComparison.Ordinal);
            if (dash >= 0)
                value = value[..dash].Trim();

            var plus = value.IndexOf('+', StringComparison.Ordinal);
            if (plus >= 0)
                value = value[..plus].Trim();

            if (value.Length >= 2 &&
                (value[0] == 'v' || value[0] == 'V') &&
                char.IsDigit(value[1]))
            {
                value = value[1..];
            }

            if (!Version.TryParse(value, out var parsed))
                return false;

            version = parsed;
            return true;
        }

        private static string? TryCallGodotOsString(string methodName)
        {
            try
            {
                var osType =
                    Type.GetType("Godot.OS, GodotSharp", false) ??
                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .Select(assembly => assembly.GetType("Godot.OS", false))
                        .FirstOrDefault(type => type is not null);

                var method = osType?.GetMethod(
                    methodName,
                    BindingFlags.Public | BindingFlags.Static);

                return method?.Invoke(null, null) as string;
            }
            catch
            {
                return null;
            }
        }
    }

    private sealed class VariantManifest
    {
        public int Schema { get; init; }

        public List<VariantFile>? Shared { get; init; }

        public List<VariantEntry>? Variants { get; init; }
    }

    private sealed class VariantEntry
    {
        public string? CompatTarget { get; init; }

        public string? Directory { get; init; }

        public List<VariantFile>? Files { get; init; }
    }

    private sealed class VariantFile
    {
        public string? Assembly { get; init; }

        public string? Sha256 { get; init; }
    }
}
