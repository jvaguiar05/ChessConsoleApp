using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;

namespace ChessGame.Core;

public class Board
{
    private readonly Piece?[,] _squares = new Piece?[8, 8];

    public Piece? this[Position position]
    {
        get => _squares[position.Row, position.Column];
        set => _squares[position.Row, position.Column] = value;
    }

    private Piece? this[int row, int col]
    {
        get => _squares[row, col];
        set => _squares[row, col] = value;
    }

    public bool IsSquareEmpty(Position position) => this[position] == null;

    public bool IsEnemyPiece(Position position, PieceColor myColor)
    {
        Piece? piece = this[position];
        return piece != null && piece.Color != myColor;
    }

    public void MovePiece(Position from, Position to)
    {
        Piece? piece = this[from];
        if (piece == null)
            return;

        // Internal En Passant side-effect management
        if (piece is Pawn && from.Column != to.Column && this[to] == null)
        {
            this[from.Row, to.Column] = null;
        }

        this[to] = piece;
        piece.SetPosition(to);
        this[from] = null;
    }

    // Helper method to locate the King of a specific color on the board
    public Position FindKing(PieceColor kingColor)
    {
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                Piece? piece = _squares[r, c];
                if (piece is King && piece.Color == kingColor)
                {
                    return new Position(r, c);
                }
            }
        }
        throw new InvalidOperationException(
            $"Critical Error: {kingColor} King not found on the board."
        );
    }

    // Determines if the King of the specified color is currently in check
    public bool IsColorInCheck(PieceColor kingColor, Move? lastMove = null)
    {
        Position kingPos = FindKing(kingColor);

        // Loop through every square to find enemy attackers
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                Piece? piece = _squares[r, c];
                if (piece != null && piece.Color != kingColor)
                {
                    // Can this specific enemy piece legally strike the King?
                    if (piece.IsValidMove(this, kingPos, lastMove))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Helper method to determine if the player has any legal moves available
    public bool HasAnyLegalMoves(
        PieceColor playerColor,
        Move? lastMove,
        Func<Position, Position, bool> simulateMoveCallback
    )
    {
        for (int rFrom = 0; rFrom < 8; rFrom++)
        {
            for (int cFrom = 0; cFrom < 8; cFrom++)
            {
                Piece? piece = _squares[rFrom, cFrom];
                if (piece == null || piece.Color != playerColor)
                    continue;

                Position from = new Position(rFrom, cFrom);

                // Test this piece against every possible square on the board
                for (int rTo = 0; rTo < 8; rTo++)
                {
                    for (int cTo = 0; cTo < 8; cTo++)
                    {
                        Position to = new Position(rTo, cTo);

                        // If a single move passes validation without leaving the king in check, they are safe
                        if (simulateMoveCallback(from, to))
                            return true;
                    }
                }
            }
        }
        return false;
    }

    public bool HasInsufficientMaterial()
    {
        var whitePieces = new List<Piece>();
        var blackPieces = new List<Piece>();

        // 1. Collect all non-King pieces on the board
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                Piece? p = _squares[r, c];
                if (p == null || p is King)
                    continue;

                if (p.Color == PieceColor.White)
                    whitePieces.Add(p);
                else
                    blackPieces.Add(p);
            }
        }

        int whiteCount = whitePieces.Count;
        int blackCount = blackPieces.Count;

        // Condition A: King vs King (No extra pieces at all)
        if (whiteCount == 0 && blackCount == 0)
            return true;

        // Condition B: King + Bishop vs King OR King + Knight vs King
        if (
            (
                whiteCount == 1
                && blackCount == 0
                && (whitePieces[0] is Bishop || whitePieces[0] is Knight)
            )
            || (
                blackCount == 1
                && whiteCount == 0
                && (blackPieces[0] is Bishop || blackPieces[0] is Knight)
            )
        )
            return true;

        // Condition C: King + Bishop vs King + Bishop
        if (
            whiteCount == 1
            && blackCount == 1
            && whitePieces[0] is Bishop
            && blackPieces[0] is Bishop
        )
        {
            // A draw only happens if BOTH bishops are locked onto the exact same square color shade!
            // (Row + Column) % 2 gives a perfect light/dark grid alternating map toggle
            bool whiteBishopIsLightSquare =
                (whitePieces[0].Position.Row + whitePieces[0].Position.Column) % 2 != 0;
            bool blackBishopIsLightSquare =
                (blackPieces[0].Position.Row + blackPieces[0].Position.Column) % 2 != 0;

            return whiteBishopIsLightSquare == blackBishopIsLightSquare;
        }

        return false;
    }

    public void Initialize()
    {
        Array.Clear(_squares, 0, _squares.Length);

        for (int col = 0; col < 8; col++)
            _squares[1, col] = new Pawn(PieceColor.White, new Position(1, col));

        _squares[0, 0] = new Rook(PieceColor.White, new Position(0, 0));
        _squares[0, 1] = new Knight(PieceColor.White, new Position(0, 1));
        _squares[0, 2] = new Bishop(PieceColor.White, new Position(0, 2));
        _squares[0, 3] = new Queen(PieceColor.White, new Position(0, 3));
        _squares[0, 4] = new King(PieceColor.White, new Position(0, 4));
        _squares[0, 5] = new Bishop(PieceColor.White, new Position(0, 5));
        _squares[0, 6] = new Knight(PieceColor.White, new Position(0, 6));
        _squares[0, 7] = new Rook(PieceColor.White, new Position(0, 7));

        for (int col = 0; col < 8; col++)
            _squares[6, col] = new Pawn(PieceColor.Black, new Position(6, col));

        _squares[7, 0] = new Rook(PieceColor.Black, new Position(7, 0));
        _squares[7, 1] = new Knight(PieceColor.Black, new Position(7, 1));
        _squares[7, 2] = new Bishop(PieceColor.Black, new Position(7, 2));
        _squares[7, 3] = new Queen(PieceColor.Black, new Position(7, 3));
        _squares[7, 4] = new King(PieceColor.Black, new Position(7, 4));
        _squares[7, 5] = new Bishop(PieceColor.Black, new Position(7, 5));
        _squares[7, 6] = new Knight(PieceColor.Black, new Position(7, 6));
        _squares[7, 7] = new Rook(PieceColor.Black, new Position(7, 7));
    }

#if DEBUG
    public void ClearForTesting() => Array.Clear(_squares);

    public void PlacePieceForTesting(Piece piece) => this[piece.Position] = piece;
#endif
}
