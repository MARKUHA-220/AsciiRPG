using AsciiRPG.Core;

namespace AsciiRPG.UI;

public class MenuSystem
{
    public int MainMenu()
    {
        Console.Clear();
        Console.WriteLine("=== ASCII DnD RPG ===");
        Console.WriteLine("1. Новая игра (DM)");
        Console.WriteLine("2. LAN: подключиться как игрок");
        Console.WriteLine("3. Редактор персонажа из сохранения");
        Console.WriteLine("4. Выход");
        Console.Write("Выбор: ");
        return int.TryParse(Console.ReadLine(), out var c) ? c : 4;
    }

    public Character BuildCharacter()
    {
        Console.Write("Имя персонажа: ");
        var name = Console.ReadLine() ?? "Hero";

        Console.WriteLine("Класс: 0-Warrior,1-Mage,2-Rogue,3-Ranger,4-Cleric");
        var cls = int.TryParse(Console.ReadLine(), out var c) ? c : 0;

        Console.WriteLine("Раса: 0-Human,1-Elf,2-Dwarf,3-Orc,4-Halfling");
        var race = int.TryParse(Console.ReadLine(), out var r) ? r : 0;

        return new Character
        {
            Name = name,
            Class = Enum.IsDefined(typeof(CharacterClass), cls) ? (CharacterClass)cls : CharacterClass.Warrior,
            Race = Enum.IsDefined(typeof(Race), race) ? (Race)race : Race.Human
        };
    }

    public void ShowReadyMenu(Dictionary<string, bool> readyFlags)
    {
        Console.WriteLine("--- Готовность игроков ---");
        foreach (var kv in readyFlags)
            Console.WriteLine($"{kv.Key}: {(kv.Value ? "READY" : "WAIT")}");
    }

    public string? SelectCharacterSave(IReadOnlyList<string> saves)
    {
        if (saves.Count == 0)
        {
            Console.WriteLine("Сохраненных персонажей не найдено.");
            return null;
        }

        Console.WriteLine("Выберите персонажа из папки saves:");
        for (var i = 0; i < saves.Count; i++)
            Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(saves[i])}");

        Console.Write("Номер (Enter — создать нового): ");
        var input = Console.ReadLine();
        if (!int.TryParse(input, out var selected)) return null;
        var index = selected - 1;
        return index >= 0 && index < saves.Count ? saves[index] : null;
    }
}
