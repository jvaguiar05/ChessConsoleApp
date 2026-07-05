using ChessConsoleApp.Enums;

namespace ChessConsoleApp.UI;

public enum MainMenuOption
{
    PlayLocal,
    PlayVsComputer,
    Exit,
}

public sealed class GameMenu
{
    public MainMenuOption ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("       CONSOLE CHESS CHAMPIONSHIP       ");
            Console.WriteLine("========================================");
            Console.WriteLine(" 1. Play Local Pass-and-Play (2P)");
            Console.WriteLine(" 2. Play vs Computer (AI)");
            Console.WriteLine(" 3. Exit Game");
            Console.Write("\nSelect an option: ");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1")
                return MainMenuOption.PlayLocal;

            if (choice == "2")
                return MainMenuOption.PlayVsComputer;

            if (choice == "3")
                return MainMenuOption.Exit;
        }
    }

    public AiDifficulty SelectDifficulty()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("       SELECT COMPUTER DIFFICULTY       ");
            Console.WriteLine("========================================");
            Console.WriteLine(" 1. Beginner     (600 Elo)");
            Console.WriteLine(" 2. Intermediate (1600 Elo)");
            Console.WriteLine(" 3. Advanced     (2000 Elo)");
            Console.WriteLine(" 4. Master       (2400 Elo)");
            Console.WriteLine(" 5. Grandmaster  (2700+ Elo)");
            Console.Write("\nSelect your opponent: ");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1")
                return AiDifficulty.Beginner;

            if (choice == "2")
                return AiDifficulty.Intermediate;

            if (choice == "3")
                return AiDifficulty.Advanced;

            if (choice == "4")
                return AiDifficulty.Master;

            if (choice == "5")
                return AiDifficulty.Grandmaster;
        }
    }

    public PieceColor SelectPlayerColor()
    {
        Console.Clear();
        Console.WriteLine("=== SELECT YOUR COLOR ===");
        Console.WriteLine(" 1. White (Moves First)");
        Console.WriteLine(" 2. Black");
        Console.Write("\nSelect an option: ");

        string choice = Console.ReadLine() ?? "";

        return choice == "2" ? PieceColor.Black : PieceColor.White;
    }
}
