using AsciiRPG.Core;
using AsciiRPG.Persistence;

namespace AsciiRPG.Gameplay;

public class GameEngine
{
    private readonly WorldGenerator _generator = new();
    private readonly CombatSystem _combat = new();
    private readonly LootSystem _loot = new();
    private readonly SaveSystem _save = new();
    private readonly Random _rng = new();

    public GameState StartNew(string dmName, List<Character> players)
    {
        var state = new GameState
        {
            DungeonMasterName = dmName,
            Map = _generator.Generate(70, 24),
            Players = players,
            ReadyFlags = players.ToDictionary(p => p.Name, _ => true)
        };

        SpawnEnemies(state, 10);
        SetupTriggers(state);
        return state;
    }

    public void Run(GameState state)
    {
        while (true)
        {
            state.TurnCounter++;
            foreach (var p in state.Players)
            {
                UpdateFogOfWar(state.Map, p);
                DrawMapForPlayer(state.Map, p);
                Console.WriteLine($"Ход {state.TurnCounter}, игрок {p.Name}");
                Console.Write("Команда (w/a/s/d, inv, equip <name>, save, quit): ");
                var cmd = (Console.ReadLine() ?? "").Trim();
                if (cmd.Equals("quit", StringComparison.OrdinalIgnoreCase)) return;
                if (cmd.Equals("save", StringComparison.OrdinalIgnoreCase))
                {
                    _save.SaveGame(state, "saves/game_save.json");
                    foreach (var player in state.Players)
                        _save.SaveCharacterToSaves(player);
                    Console.WriteLine("Игра и персонажи сохранены в папку saves");
                    continue;
                }

                HandleCommand(state, p, cmd);
                ProcessTile(state, p);
                _save.SaveCharacterToSaves(p);

                if (state.Map.GetTile(p.Position).Type == TileType.Exit)
                {
                    Console.WriteLine("Победа! Вы нашли выход из подземелья.");
                    return;
                }

                if (p.HitPoints <= 0)
                {
                    Console.WriteLine($"{p.Name} пал. Игра окончена.");
                    return;
                }
            }
        }
    }

    private void HandleCommand(GameState state, Character p, string cmd)
    {
        var normalized = cmd.Trim();
        if (normalized.Length == 0) return;

        if (normalized.StartsWith("equip ", StringComparison.OrdinalIgnoreCase))
        {
            p.Inventory.Equip(normalized[6..].Trim());
            return;
        }

        if (normalized.Equals("inv", StringComparison.OrdinalIgnoreCase) || normalized.Equals("i", StringComparison.OrdinalIgnoreCase) || normalized.Equals("инв", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Инвентарь:");
            if (p.Inventory.Items.Count == 0)
            {
                Console.WriteLine("- Пусто");
                return;
            }

            foreach (var item in p.Inventory.Items)
            {
                var equipped = p.Inventory.EquippedWeapon == item || p.Inventory.EquippedArmor == item ? " [экипировано]" : string.Empty;
                Console.WriteLine($"- {item.Name}{equipped}");
            }
            return;
        }

        var delta = normalized.ToLowerInvariant() switch
        {
            "w" or "ц" => new Position(0, -1),
            "s" or "ы" => new Position(0, 1),
            "a" or "ф" => new Position(-1, 0),
            "d" or "в" => new Position(1, 0),
            _ => new Position(0, 0)
        };

        if (delta == new Position(0, 0))
        {
            Console.WriteLine("Неизвестная команда.");
            return;
        }

        var np = new Position(p.Position.X + delta.X, p.Position.Y + delta.Y);
        if (!state.Map.IsInBounds(np))
        {
            Console.WriteLine("Дальше идти нельзя: граница карты.");
            return;
        }

        var tile = state.Map.GetTile(np);
        if (tile.Type == TileType.Wall)
        {
            Console.WriteLine("Там стена.");
            return;
        }

        p.Position = np;
    }

    private void ProcessTile(GameState state, Character p)
    {
        var tile = state.Map.GetTile(p.Position);
        switch (tile.Type)
        {
            case TileType.Chest:
                var loot = _loot.RollLoot(p.Level);
                p.Inventory.Items.Add(loot);
                Console.WriteLine($"Сундук: получен предмет {loot.Name}");
                tile.Type = TileType.Room;
                break;
            case TileType.Trigger:
                p.Statuses.Add(new StatusEffect { Type = StatusType.Blessed, Duration = 3 });
                Console.WriteLine("Триггер: на вас наложен статус Blessed на 3 хода.");
                tile.Type = TileType.Room;
                break;
            case TileType.EnemySpawn:
                var enemy = state.Enemies.FirstOrDefault(e => e.Position == p.Position) ?? RandomEnemyAt(p.Position);
                var win = _combat.ResolveBattle(p, enemy);
                if (win)
                {
                    state.Enemies.Remove(enemy);
                    Console.WriteLine("Враг повержен.");
                    tile.Type = TileType.Room;
                }
                break;
        }
    }

    private Enemy RandomEnemyAt(Position pos) => new()
    {
        Name = new[] { "Goblin", "Skeleton", "Bandit", "Cultist" }[_rng.Next(4)],
        Position = pos,
        HitPoints = _rng.Next(10, 20),
        Attack = _rng.Next(3, 7),
        Defense = _rng.Next(1, 4),
        Initiative = _rng.Next(5, 12)
    };

    private void SpawnEnemies(GameState state, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var x = _rng.Next(1, state.Map.Width - 1);
            var y = _rng.Next(1, state.Map.Height - 1);
            if (state.Map.Grid[y, x].Type is TileType.Room or TileType.Corridor)
            {
                state.Map.Grid[y, x].Type = TileType.EnemySpawn;
                state.Enemies.Add(RandomEnemyAt(new Position(x, y)));
            }
        }
    }

    private void SetupTriggers(GameState state)
    {
        state.Triggers.Add(new TriggerEvent
        {
            Position = new Position(2, 2),
            Description = "Магический алтарь лечит на 5 HP",
            Effect = c => c.HitPoints = Math.Min(c.MaxHitPoints, c.HitPoints + 5)
        });
    }

    private static void UpdateFogOfWar(WorldMap map, Character p)
    {
        for (var y = p.Position.Y - 3; y <= p.Position.Y + 3; y++)
        for (var x = p.Position.X - 3; x <= p.Position.X + 3; x++)
        {
            var pos = new Position(x, y);
            if (!map.IsInBounds(pos)) continue;
            p.DiscoveredTiles.Add($"{x}:{y}");
        }
    }

    private static void DrawMapForPlayer(WorldMap map, Character p)
    {
        Console.Clear();
        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                var key = $"{x}:{y}";
                if (!p.DiscoveredTiles.Contains(key))
                {
                    Console.Write('?');
                    continue;
                }

                if (p.Position.X == x && p.Position.Y == y)
                {
                    Console.Write('@');
                    continue;
                }

                Console.Write(map.Grid[y, x].Type switch
                {
                    TileType.Wall => '#',
                    TileType.Room => '.',
                    TileType.Corridor => '=',
                    TileType.Chest => 'C',
                    TileType.Trigger => 'T',
                    TileType.EnemySpawn => 'E',
                    TileType.Exit => 'X',
                    _ => ' '
                });
            }
            Console.WriteLine();
        }
    }
}
