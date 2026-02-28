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
                Console.WriteLine($"Turn {state.TurnCounter}, player {p.Name}");
                Console.Write("Command (w/a/s/d, inv, equip <name>, save, quit): ");
                var cmd = Console.ReadLine() ?? "";
                if (cmd == "quit") return;
                if (cmd == "save")
                {
                    _save.SaveGame(state, "saves/game_save.json");
                    Console.WriteLine("Game saved to saves/game_save.json");
                    continue;
                }

                HandleCommand(state, p, cmd);
                ProcessTile(state, p);

                if (state.Map.GetTile(p.Position).Type == TileType.Exit)
                {
                    Console.WriteLine("Victory! You found the dungeon exit.");
                    return;
                }

                if (p.HitPoints <= 0)
                {
                    Console.WriteLine($"{p.Name} has fallen. Game over.");
                    return;
                }
            }
        }
    }

    private void HandleCommand(GameState state, Character p, string cmd)
    {
        var delta = cmd switch
        {
            "w" => new Position(0, -1),
            "s" => new Position(0, 1),
            "a" => new Position(-1, 0),
            "d" => new Position(1, 0),
            _ => new Position(0, 0)
        };

        if (cmd.StartsWith("equip ", StringComparison.OrdinalIgnoreCase))
        {
            p.Inventory.Equip(cmd[6..]);
            return;
        }

        if (cmd == "inv")
        {
            Console.WriteLine("Inventory:");
            foreach (var item in p.Inventory.Items) Console.WriteLine($"- {item.Name}");
            return;
        }

        var np = new Position(p.Position.X + delta.X, p.Position.Y + delta.Y);
        if (!state.Map.IsInBounds(np)) return;
        var tile = state.Map.GetTile(np);
        if (tile.Type == TileType.Wall) return;
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
                Console.WriteLine($"Chest: found item {loot.Name}");
                tile.Type = TileType.Room;
                break;
            case TileType.Trigger:
                p.Statuses.Add(new StatusEffect { Type = StatusType.Blessed, Duration = 3 });
                Console.WriteLine("Trigger: Blessed status applied for 3 turns.");
                tile.Type = TileType.Room;
                break;
            case TileType.EnemySpawn:
                var enemy = state.Enemies.FirstOrDefault(e => e.Position == p.Position) ?? RandomEnemyAt(p.Position);
                var win = _combat.ResolveBattle(p, enemy);
                if (win)
                {
                    state.Enemies.Remove(enemy);
                    Console.WriteLine("Enemy defeated.");
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
            Description = "Magic altar heals 5 HP",
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
