using AsciiRPG.Core;

namespace AsciiRPG.UI;

public class MenuSystem
{
    public int MainMenu()
    {
        Console.Clear();
        Console.WriteLine("=== ASCII DnD RPG ===");
        Console.WriteLine("1. New game (DM)");
        Console.WriteLine("2. LAN: join as player");
        Console.WriteLine("3. Character save editor");
        Console.WriteLine("4. Exit");
        Console.Write("Choice: ");
        return int.TryParse(Console.ReadLine(), out var c) ? c : 4;
    }

    public Character BuildCharacter()
    {
        Console.Write("Character name: ");
        var name = Console.ReadLine() ?? "Hero";

        Console.WriteLine("Class: 0-Warrior,1-Mage,2-Rogue,3-Ranger,4-Cleric");
        var cls = int.TryParse(Console.ReadLine(), out var c) ? c : 0;

        Console.WriteLine("Race: 0-Human,1-Elf,2-Dwarf,3-Orc,4-Halfling");
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
        Console.WriteLine("--- Player readiness ---");
        foreach (var kv in readyFlags)
            Console.WriteLine($"{kv.Key}: {(kv.Value ? "READY" : "WAIT")}");
    }
}
