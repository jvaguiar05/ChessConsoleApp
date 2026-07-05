using System.Text;
using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Engine;

public static class FenConverter
{
    public static string ToFen(GameSession session)
    {
        string piecePlacement = BuildPiecePlacement(session);
        string activeColor = session.CurrentTurn == PieceColor.White ? "w" : "b";
        string castlingRights = BuildCastlingRights(session);
        string enPassantTarget = BuildEnPassantTarget(session);
        string halfMoveClock = session.HalfMoveClock.ToString();
        string fullMoveNumber = BuildFullMoveNumber(session).ToString();

        return $"{piecePlacement} {activeColor} {castlingRights} {enPassantTarget} {halfMoveClock} {fullMoveNumber}";
    }

    private static string BuildPiecePlacement(GameSession session)
    {
        var fen = new StringBuilder();

        for (int row = 7; row >= 0; row--)
        {
            int emptySquares = 0;

            for (int column = 0; column < 8; column++)
            {
                Piece? piece = session.Board[new Position(row, column)];

                if (piece == null)
                {
                    emptySquares++;
                    continue;
                }

                if (emptySquares > 0)
                {
                    fen.Append(emptySquares);
                    emptySquares = 0;
                }

                fen.Append(GetFenSymbol(piece));
            }

            if (emptySquares > 0)
                fen.Append(emptySquares);

            if (row > 0)
                fen.Append('/');
        }

        return fen.ToString();
    }

    private static char GetFenSymbol(Piece piece)
    {
        char symbol = piece switch
        {
            Pawn => 'p',
            Knight => 'n',
            Bishop => 'b',
            Rook => 'r',
            Queen => 'q',
            King => 'k',
            _ => throw new InvalidOperationException($"Unknown piece type: {piece.GetType().Name}"),
        };

        return piece.Color == PieceColor.White ? char.ToUpper(symbol) : symbol;
    }

    private static string BuildCastlingRights(GameSession session)
    {
        var rights = new StringBuilder();

        AddCastlingRight(session, PieceColor.White, kingSide: true, rights, 'K');
        AddCastlingRight(session, PieceColor.White, kingSide: false, rights, 'Q');
        AddCastlingRight(session, PieceColor.Black, kingSide: true, rights, 'k');
        AddCastlingRight(session, PieceColor.Black, kingSide: false, rights, 'q');

        return rights.Length == 0 ? "-" : rights.ToString();
    }

    private static void AddCastlingRight(
        GameSession session,
        PieceColor color,
        bool kingSide,
        StringBuilder rights,
        char fenSymbol
    )
    {
        int row = color == PieceColor.White ? 0 : 7;
        int rookColumn = kingSide ? 7 : 0;

        Position kingPosition = new Position(row, 4);
        Position rookPosition = new Position(row, rookColumn);

        Piece? king = session.Board[kingPosition];
        Piece? rook = session.Board[rookPosition];

        if (king is not King || king.Color != color)
            return;

        if (rook is not Rook || rook.Color != color)
            return;

        if (king.HasMoved(session.MoveHistory) || rook.HasMoved(session.MoveHistory))
            return;

        rights.Append(fenSymbol);
    }

    private static string BuildEnPassantTarget(GameSession session)
    {
        if (session.LastMove is not Move lastMove)
            return "-";

        if (lastMove.PieceMoved is not Pawn)
            return "-";

        int rowDifference = Math.Abs(lastMove.From.Row - lastMove.To.Row);

        if (rowDifference != 2)
            return "-";

        int targetRow = (lastMove.From.Row + lastMove.To.Row) / 2;
        Position target = new(targetRow, lastMove.From.Column);

        return target.ToString();
    }

    private static int BuildFullMoveNumber(GameSession session) =>
        (session.MoveHistory.Count / 2) + 1;
}
