using ChessConsoleApp.Core;
using ChessConsoleApp.Tests;

namespace ChessConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.Title = "Console Chess Core Engine";

        if (args.Length > 0 && args[0] == "--run-tests")
        {
            RunAutomatedTests();
            return;
        }

        var engine = new GameEngine();
        engine.ShowMainMenu();
    }

    private static void RunAutomatedTests()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================");
        Console.WriteLine("        LAUNCHING ENGINE TEST SUITE     ");
        Console.WriteLine("========================================\n");
        Console.ResetColor();

        bool allPassed = TestSuite.RunAll();

        Console.WriteLine("\n----------------------------------------");

        Console.ForegroundColor = allPassed ? ConsoleColor.Green : ConsoleColor.Red;

        Console.WriteLine(
            allPassed
                ? "[SUCCESS] All targeted test suites passed cleanly!"
                : "[FAILURE] Test suite regressions detected. Review code diagnostics."
        );

        Console.ResetColor();

        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Press any key to close test runner...");
        Console.ReadKey();
    }
}
