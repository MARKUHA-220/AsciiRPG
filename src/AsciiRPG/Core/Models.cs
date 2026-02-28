using System.Text.Json.Serialization;

namespace AsciiRPG.Core;

public enum TileType { Wall, Corridor, Room, Chest, Trigger, EnemySpawn, Exit }
public enum CharacterClass { Warrior, Mage, Rogue, Ranger, Cleric }
public enum Race { Human, Elf, Dwarf, Orc, Halfling }
public enum ItemType { Weapon, Armor, Consumable, Quest }
public enum StatusType { Poisoned, Bleeding, Shielded, Blessed, Stunned }

public record Position(int X, int Y);

public class Tile
{
    public TileType Type { get; set; } = TileType.Wall;
    public string Modifier { get; set; } = "Normal";
    public bool IsDiscovered { get; set; }
}

public class WorldMap
{
    public int Width { get; init; }
    public int Height { get; init; }
    public Tile[,] Grid { get; init; }

    public WorldMap(int width, int height)
    {
        Width = width;
        Height = height;
        Grid = new Tile[height, width];
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                Grid[y, x] = new Tile();
    }

    public bool IsInBounds(Position p) => p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height;
    public Tile GetTile(Position p) => Grid[p.Y, p.X];
}

public class Item
{
    public string Name { get; set; } = "Unknown";
    public ItemType Type { get; set; } = ItemType.Consumable;
    public int Power { get; set; }
}

public class Inventory
{
    public List<Item> Items { get; set; } = [];
    public Item? EquippedWeapon { get; set; }
    public Item? EquippedArmor { get; set; }

    public void Equip(string itemName)
    {
        var item = Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        if (item is null) return;
        if (item.Type == ItemType.Weapon) EquippedWeapon = item;
        if (item.Type == ItemType.Armor) EquippedArmor = item;
    }
}

public class StatusEffect
{
    public StatusType Type { get; set; }
    public int Duration { get; set; }
}

public class Character
{
    public string Name { get; set; } = "Hero";
    public CharacterClass Class { get; set; } = CharacterClass.Warrior;
    public Race Race { get; set; } = Race.Human;
    public int Level { get; set; } = 1;
    public int HitPoints { get; set; } = 30;
    public int MaxHitPoints { get; set; } = 30;
    public int Attack { get; set; } = 5;
    public int Defense { get; set; } = 2;
    public int Initiative { get; set; } = 10;
    public Position Position { get; set; } = new(1, 1);
    public Inventory Inventory { get; set; } = new();
    public List<StatusEffect> Statuses { get; set; } = [];
    [JsonIgnore]
    public HashSet<string> DiscoveredTiles { get; set; } = [];

    public int EffectiveAttack => Attack + (Inventory.EquippedWeapon?.Power ?? 0);
    public int EffectiveDefense => Defense + (Inventory.EquippedArmor?.Power ?? 0);
}

public class Enemy
{
    public string Name { get; set; } = "Goblin";
    public int HitPoints { get; set; } = 15;
    public int Attack { get; set; } = 4;
    public int Defense { get; set; } = 1;
    public int Initiative { get; set; } = 8;
    public Position Position { get; set; } = new(0, 0);
}

public class TriggerEvent
{
    public Position Position { get; set; } = new(0, 0);
    public string Description { get; set; } = "Ancient rune pulses.";
    public Action<Character>? Effect { get; set; }
}

public class GameState
{
    public string DungeonMasterName { get; set; } = "DM";
    public WorldMap Map { get; set; } = new(50, 20);
    public List<Character> Players { get; set; } = [];
    public List<Enemy> Enemies { get; set; } = [];
    public List<TriggerEvent> Triggers { get; set; } = [];
    public Dictionary<string, bool> ReadyFlags { get; set; } = [];
    public int TurnCounter { get; set; }
}
