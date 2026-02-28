using System.Text.Json;
using AsciiRPG.Core;

namespace AsciiRPG.Persistence;

public class SaveSystem
{
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

    public void SaveGame(GameState game, string path)
    {
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
        File.WriteAllText(path, JsonSerializer.Serialize(character, _opts));
    }

    public Character? LoadCharacter(string path)
    {
        if (!File.Exists(path)) return null;
        return JsonSerializer.Deserialize<Character>(File.ReadAllText(path));
    }
}
