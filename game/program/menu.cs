namespace AsciiRPG.Game.Program;

internal sealed class Menu
{
    public string? ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("=== ASCII RPG ===");
        Console.WriteLine("1. Новая игра");
        Console.WriteLine("2. Загрузить игру");
        Console.WriteLine("3. Выход");
        Console.Write("Выбери пункт (1-3): ");

        return Console.ReadLine()?.Trim();
    }

    public static bool IsExitCommand(string? command)
    {
        return command is "3" or "q" or "Q";
    }
}