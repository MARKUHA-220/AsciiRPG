using AsciiRPG.Core;

namespace AsciiRPG.Gameplay;

public class LootSystem
{
    private readonly Random _rng = new();

    public Item RollLoot(int playerLevel)
    {
        var tier = Math.Max(1, playerLevel + _rng.Next(-1, 2));
        var roll = _rng.Next(100);
        return roll switch
        {
            < 40 => new Item { Name = $"Sword +{tier}", Type = ItemType.Weapon, Power = tier + 1 },
            < 70 => new Item { Name = $"Armor +{tier}", Type = ItemType.Armor, Power = tier },
            < 90 => new Item { Name = "Healing Potion", Type = ItemType.Consumable, Power = 10 + tier * 2 },
            _ => new Item { Name = "Ancient Relic", Type = ItemType.Quest, Power = tier * 3 }
        };
    }
}
