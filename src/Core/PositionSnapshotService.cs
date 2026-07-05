using System.Text;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Core;

public sealed class PositionSnapshotService
{
    public static string Generate(GameSession gameState)
    {
        var sb = new StringBuilder();

        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                Piece? piece = gameState.Board[new Position(row, column)];

                if (piece == null)
                {
                    sb.Append('.');
                    continue;
                }

                char symbol = piece is Knight ? 'N' : piece.GetType().Name[0];

                sb.Append(
                    piece.Color == PieceColor.White ? char.ToUpper(symbol) : char.ToLower(symbol)
                );
            }
        }

        sb.Append($"|{gameState.CurrentTurn}");

        return sb.ToString();
    }

    public static void RegisterCurrentPosition(GameSession gameState)
    {
        string snapshot = Generate(gameState);

        if (gameState.PositionHistory.TryGetValue(snapshot, out int count))
            gameState.PositionHistory[snapshot] = count + 1;
        else
            gameState.PositionHistory[snapshot] = 1;
    }
}
