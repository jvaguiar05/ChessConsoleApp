using ChessConsoleApp.Enums;

namespace ChessConsoleApp.UI;

public static class PromotionPrompt
{
    public static char AskPromotionChoice(PieceColor color)
    {
        Console.WriteLine(
            $"\nPawn Promotion! Choose a piece for {color} (Q = Queen, R = Rook, B = Bishop, N = Knight):"
        );

        string choice = Console.ReadLine()?.Trim().ToUpper() ?? "Q";

        if (string.IsNullOrWhiteSpace(choice))
            return 'Q';

        return choice[0];
    }
}
