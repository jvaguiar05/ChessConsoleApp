using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;

namespace ChessConsoleApp.UI;

public static class EndGameRenderer
{
    public static void Render(GameSession session)
    {
        switch (session.State)
        {
            case GameState.Checkmate:
                PieceColor winner =
                    session.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

                Console.WriteLine($"\n[!!!] CHECKMATE! {winner} wins the game!");
                break;

            case GameState.Stalemate:
                Console.WriteLine(
                    "\n[---] STALEMATE! The active player has no legal moves. The game is a draw."
                );
                break;

            case GameState.Resigned:
                PieceColor victor =
                    session.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

                Console.WriteLine(
                    $"\n[x] RESIGNATION! {session.CurrentTurn} threw in the towel. {victor} wins the match!"
                );
                break;

            case GameState.DrawByFiftyMoveRule:
                Console.WriteLine(
                    "\n[---] DRAW! The fifty-move rule has been invoked. No pawn moves or captures in the last fifty moves."
                );
                break;

            case GameState.DrawByRepetition:
                Console.WriteLine(
                    "\n[---] DRAW! Threefold Repetition detected. The exact same board position has occurred three times."
                );
                break;

            case GameState.DrawByInsufficientMaterial:
                Console.WriteLine(
                    "\n[---] DRAW! Insufficient Material detected. Neither side has enough remaining pieces to force a legal checkmate."
                );
                break;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nPress any key to return to the Main Menu...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
