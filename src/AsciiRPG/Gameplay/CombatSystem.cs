using AsciiRPG.Core;

namespace AsciiRPG.Gameplay;

public class CombatSystem
{
    private readonly Random _rng = new();

    public bool ResolveBattle(Character player, Enemy enemy)
    {
        Console.WriteLine($"Battle: {player.Name} vs {enemy.Name}");
        Console.WriteLine($"Бой: {player.Name} vs {enemy.Name}");
        while (player.HitPoints > 0 && enemy.HitPoints > 0)
        {
            var order = new List<(string who, int initiative)>
            {
                ("player", player.Initiative + _rng.Next(1, 7)),
                ("enemy", enemy.Initiative + _rng.Next(1, 7))
            }.OrderByDescending(x => x.initiative);

            foreach (var actor in order)
            {
                if (player.HitPoints <= 0 || enemy.HitPoints <= 0) break;
                if (actor.who == "player")
                {
                    var damage = Math.Max(1, player.EffectiveAttack - enemy.Defense + _rng.Next(-1, 3));
                    enemy.HitPoints -= damage;
                    Console.WriteLine($"{player.Name} deals {damage}. Enemy HP: {Math.Max(0, enemy.HitPoints)}");
                    Console.WriteLine($"{player.Name} наносит {damage}. HP врага: {Math.Max(0, enemy.HitPoints)}");
                }
                else
                {
                    var damage = Math.Max(1, enemy.Attack - player.EffectiveDefense + _rng.Next(-1, 3));
                    player.HitPoints -= damage;
                    Console.WriteLine($"{enemy.Name} deals {damage}. Player HP: {Math.Max(0, player.HitPoints)}");
                    Console.WriteLine($"{enemy.Name} наносит {damage}. HP игрока: {Math.Max(0, player.HitPoints)}");
                }
            }

            TickStatuses(player);
        }

        return player.HitPoints > 0;
    }

    private void TickStatuses(Character player)
    {
        foreach (var status in player.Statuses.ToList())
        {
            if (status.Type == StatusType.Poisoned)
            {
                player.HitPoints = Math.Max(0, player.HitPoints - 2);
                Console.WriteLine($"{player.Name} takes 2 poison damage.");
                Console.WriteLine($"{player.Name} получает 2 урона от яда.");
            }

            status.Duration--;
            if (status.Duration <= 0) player.Statuses.Remove(status);
        }
    }
}
