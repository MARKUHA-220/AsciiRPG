using AsciiRPG.Core;

namespace AsciiRPG.Gameplay;

public class WorldGenerator
{
    private readonly Random _rng = new();

    public WorldMap Generate(int width, int height, int roomCount = 8)
    {
        var map = new WorldMap(width, height);
        var rooms = new List<(int x, int y, int w, int h)>();

        for (var i = 0; i < roomCount; i++)
        {
            var w = _rng.Next(4, 10);
            var h = _rng.Next(4, 8);
            var x = _rng.Next(1, width - w - 1);
            var y = _rng.Next(1, height - h - 1);
            rooms.Add((x, y, w, h));

            for (var yy = y; yy < y + h; yy++)
            for (var xx = x; xx < x + w; xx++)
            {
                map.Grid[yy, xx].Type = TileType.Room;
                map.Grid[yy, xx].Modifier = PickModifier();
            }
        }

        for (var i = 1; i < rooms.Count; i++)
        {
            CarveTunnel(map, Center(rooms[i - 1]), Center(rooms[i]));
        }

        Decorate(map, rooms);
        return map;
    }

    private static (int x, int y) Center((int x, int y, int w, int h) room)
        => (room.x + room.w / 2, room.y + room.h / 2);

    private void CarveTunnel(WorldMap map, (int x, int y) a, (int x, int y) b)
    {
        var x = a.x;
        var y = a.y;
        while (x != b.x)
        {
            x += Math.Sign(b.x - x);
            map.Grid[y, x].Type = TileType.Corridor;
        }

        while (y != b.y)
        {
            y += Math.Sign(b.y - y);
            map.Grid[y, x].Type = TileType.Corridor;
        }
    }

    private void Decorate(WorldMap map, List<(int x, int y, int w, int h)> rooms)
    {
        foreach (var room in rooms)
        {
            map.Grid[room.y + 1, room.x + 1].Type = TileType.Chest;
            map.Grid[room.y + room.h - 2, room.x + room.w - 2].Type = TileType.Trigger;
            map.Grid[room.y + room.h / 2, room.x + room.w / 2].Type = TileType.EnemySpawn;
        }

        var exitRoom = rooms.Last();
        map.Grid[exitRoom.y + exitRoom.h / 2, exitRoom.x + exitRoom.w / 2].Type = TileType.Exit;
    }

    private string PickModifier()
    {
        var modifiers = new[] { "Blessed", "Cursed", "Foggy", "Sacred", "Dark", "Icy" };
        return modifiers[_rng.Next(modifiers.Length)];
    }
}
