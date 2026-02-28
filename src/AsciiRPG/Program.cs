using AsciiRPG.Gameplay;
using AsciiRPG.Network;
using AsciiRPG.Persistence;
using AsciiRPG.UI;

var menu = new MenuSystem();
var save = new SaveSystem();
var engine = new GameEngine();
var lan = new LanSession();

while (true)
{
    var choice = menu.MainMenu();
    switch (choice)
    {
        case 1:
            Console.Write("Имя DM: ");
            var dm = Console.ReadLine() ?? "DM";
            Console.Write("Запустить LAN-хост для флагов готовности? (y/n): ");
            var runHost = (Console.ReadLine() ?? "n").Equals("y", StringComparison.OrdinalIgnoreCase);
            CancellationTokenSource? cts = null;
            if (runHost)
            {
                Console.Write("Порт LAN-хоста: ");
                var hostPort = int.TryParse(Console.ReadLine(), out var hp) ? hp : 5050;
                cts = new CancellationTokenSource();
                _ = lan.StartHostAsync(hostPort, cts.Token);
            }

            var players = new List<AsciiRPG.Core.Character>();
            Console.Write("Сколько локальных игроков (1-4): ");
            var count = int.TryParse(Console.ReadLine(), out var c) ? Math.Clamp(c, 1, 4) : 1;
            for (var i = 0; i < count; i++)
            {
                Console.WriteLine($"Создание персонажа #{i + 1}");
                players.Add(menu.BuildCharacter());
            }

            var state = engine.StartNew(dm, players);
            menu.ShowReadyMenu(state.ReadyFlags);
            engine.Run(state);
            cts?.Cancel();
            break;

        case 2:
            Console.Write("Host IP: ");
            var host = Console.ReadLine() ?? "127.0.0.1";
            Console.Write("Port: ");
            var port = int.TryParse(Console.ReadLine(), out var p) ? p : 5050;
            Console.Write("Player name: ");
            var nick = Console.ReadLine() ?? "Player";
            await lan.JoinAsync(host, port, nick);
            Console.WriteLine("Нажмите Enter для возврата в меню");
            Console.ReadLine();
            break;

        case 3:
            Console.Write("Путь к сохранению персонажа (например saves/character.json): ");
            var path = Console.ReadLine() ?? "saves/character.json";
            var character = save.LoadCharacter(path) ?? menu.BuildCharacter();
            Console.WriteLine($"Текущий персонаж: {character.Name} ({character.Race} {character.Class})");
            Console.Write("Новое имя (Enter чтобы оставить): ");
            var newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName)) character.Name = newName;
            save.SaveCharacter(character, path);
            Console.WriteLine($"Сохранено: {path}");
            Console.WriteLine("Нажмите Enter для возврата в меню");
            Console.ReadLine();
            break;

        default:
            return;
    }
}
