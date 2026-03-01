using System.Text.Json;
using AsciiRPG.Core;

namespace AsciiRPG.Persistence;

public class SaveSystem
{
    public const string SavesDirectory = "saves";
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

    public void SaveGame(GameState game, string path)
    {
        EnsureDirectory(path);
        var json = JsonSerializer.Serialize(game, _opts);
        File.WriteAllText(path, json);
    }

    public GameState? LoadGame(string path)
    {
        if (!File.Exists(path)) return null;
        return JsonSerializer.Deserialize<GameState>(File.ReadAllText(path));
    }

    public void SaveCharacter(Character character, string path)
    {
        EnsureDirectory(path);
        File.WriteAllText(path, JsonSerializer.Serialize(character, _opts));
    }

    public string SaveCharacterToSaves(Character character)
    {
        Directory.CreateDirectory(SavesDirectory);
        var safeName = string.Concat(character.Name
            .Trim()
            .Select(ch => Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch));
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "character";

        var path = Path.Combine(SavesDirectory, $"{safeName}.json");
        SaveCharacter(character, path);
        return path;
    }

    public IReadOnlyList<string> GetCharacterSaves()
    {
        Directory.CreateDirectory(SavesDirectory);
        return Directory.GetFiles(SavesDirectory, "*.json")
            .OrderBy(Path.GetFileName)
            .ToList();
    }

    private static void EnsureDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
    }

    public Character? LoadCharacter(string path)
    {
        if (!File.Exists(path)) return null;
        return JsonSerializer.Deserialize<Character>(File.ReadAllText(path));
    }
}
