using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public class CardValidator : Task
{
    [Required]
    public string CardsDir { get; set; }

    public override bool Execute()
    {
        if (!Directory.Exists(CardsDir))
        {
            Log.LogWarning($"Cards directory not found: {CardsDir}");
            return true; // 不存在时不视为失败
        }

        bool allValid = true;
        foreach (string file in Directory.GetFiles(CardsDir, "*.cs"))
        {
            if (file.EndsWith(".cs.uid", StringComparison.OrdinalIgnoreCase))
                continue;

            string content = File.ReadAllText(file);
            string fileName = Path.GetFileNameWithoutExtension(file);

            // 1. 必须存在继承自 ModCardTemplate 的 public class
            Match classMatch = Regex.Match(content, 
                @"public\s+class\s+(\w+)\s*:\s*[^}]*\bModCardTemplate\b");
            if (!classMatch.Success)
            {
                Log.LogError($"Validation failed in {file}: no public class inheriting from ModCardTemplate.");
                allValid = false;
                continue;
            }

            string className = classMatch.Groups[1].Value;

            // 2. 类名必须与文件名一致（可选，但容易实现）
            if (!string.Equals(className, fileName, StringComparison.Ordinal))
            {
                Log.LogError($"Validation failed in {file}: class name '{className}' doesn't match file name '{fileName}'.");
                allValid = false;
            }

            // 3. Strike 规则检查
            bool nameContainsStrike = className.IndexOf("Strike", StringComparison.OrdinalIgnoreCase) >= 0;
            bool hasOverride = Regex.IsMatch(content, @"override\s+.*\bCanonicalTags\b");
            bool hasStrikeTag = false;
            if (hasOverride)
            {
                Match overrideBlock = Regex.Match(content, 
                    @"override\s+.*CanonicalTags\b\s*(?:=>\s*(?<expr>[^;]*);|\{(?<body>[^}]*)\})");
                if (overrideBlock.Success)
                {
                    string text = overrideBlock.Groups["expr"].Success 
                        ? overrideBlock.Groups["expr"].Value 
                        : overrideBlock.Groups["body"].Value;
                    hasStrikeTag = Regex.IsMatch(text, @"\bCardTag\.Strike\b");
                }
            }

            if (nameContainsStrike)
            {
                if (!hasOverride || !hasStrikeTag)
                {
                    Log.LogError($"Validation failed in {file}: class '{className}' contains 'Strike' but doesn't override CanonicalTags with CardTag.Strike.");
                    allValid = false;
                }
            }
            else
            {
                if (hasOverride && hasStrikeTag)
                {
                    Log.LogError($"Validation failed in {file}: class '{className}' doesn't contain 'Strike' but overrides CanonicalTags with CardTag.Strike.");
                    allValid = false;
                }
            }
        }
        return allValid;
    }
}