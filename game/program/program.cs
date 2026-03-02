using AsciiRPG.Game.Program;

var menu = new Menu();

while (true)
{
    var command = menu.ShowMainMenu();

    if (Menu.IsExitCommand(command))
    {
        Console.WriteLine("До встречи в ASCII RPG!");
        break;
    }

    switch (command)
    {
        case "1":
            Console.WriteLine("Новая игра скоро будет доступна.");
            break;
        case "2":
            Console.WriteLine("Загрузка игры скоро будет доступна.");
            break;
        default:
            Console.WriteLine("Неизвестная команда. Попробуй ещё раз.");
            break;
    }

    Console.WriteLine("Нажми Enter, чтобы продолжить...");
    Console.ReadLine();
}