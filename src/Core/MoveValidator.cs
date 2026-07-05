using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Core;

public sealed class MoveValidator
{
    public MoveValidationResult Validate(GameSession session, Position from, Position to)
    {
        Piece? targetCaptured = session.Board[to];

        bool isEnPassant = false;
        bool isCastling = false;
        Position rookStartPosition = default;
        Position rookTransitPosition = default;
        Piece? castlingRook = null;

        Piece? piece = session.Board[from];

        if (piece == null)
            return MoveValidationResult.Illegal(
                from,
                to,
                "Selection error: No piece exists at the specified source coordinate."
            );

        if (piece.Color != session.CurrentTurn)
            return MoveValidationResult.Illegal(
                from,
                to,
                $"Turn error: It is currently {session.CurrentTurn}'s turn."
            );

        Move? lastMove = session.LastMove;

        if (!piece.IsValidMove(session.Board, to, lastMove))
            return MoveValidationResult.Illegal(
                from,
                to,
                $"Movement error: Invalid rule trajectory for {piece.GetType().Name}."
            );

        isEnPassant = piece is Pawn && from.Column != to.Column && targetCaptured == null;
        Position enPassantVictimPosition = new Position(from.Row, to.Column);
        Piece? enPassantVictim = isEnPassant ? session.Board[enPassantVictimPosition] : null;

        if (isEnPassant)
            targetCaptured = enPassantVictim;

        isCastling = piece is King && Math.Abs(from.Column - to.Column) == 2;

        if (isCastling)
        {
            if (piece.HasMoved(session.MoveHistory))
                return MoveValidationResult.Illegal(
                    from,
                    to,
                    "Castling error: The king has already moved."
                );

            if (session.Board.IsColorInCheck(session.CurrentTurn, lastMove))
                return MoveValidationResult.Illegal(
                    from,
                    to,
                    "Castling error: The king cannot castle while in check."
                );

            int rookStartColumn = to.Column == 6 ? 7 : 0;
            int rookTransitColumn = to.Column == 6 ? 5 : 3;

            rookStartPosition = new Position(from.Row, rookStartColumn);
            rookTransitPosition = new Position(from.Row, rookTransitColumn);
            castlingRook = session.Board[rookStartPosition];

            if (castlingRook == null || castlingRook.HasMoved(session.MoveHistory))
                return MoveValidationResult.Illegal(
                    from,
                    to,
                    "Castling error: The rook is unavailable or has already moved."
                );

            int step = to.Column == 6 ? 1 : -1;
            Position transitPosition = new Position(from.Row, from.Column + step);

            session.Board[transitPosition] = piece;
            session.Board[from] = null;
            piece.SetPosition(transitPosition);

            bool transitInCheck = session.Board.IsColorInCheck(session.CurrentTurn, lastMove);

            session.Board[from] = piece;
            session.Board[transitPosition] = null;
            piece.SetPosition(from);

            if (transitInCheck)
                return MoveValidationResult.Illegal(
                    from,
                    to,
                    "Castling error: The king cannot pass through check."
                );
        }

        session.Board[to] = piece;
        session.Board[from] = null;
        piece.SetPosition(to);

        if (isEnPassant)
            session.Board[enPassantVictimPosition] = null;

        bool selfInCheck = session.Board.IsColorInCheck(session.CurrentTurn, lastMove);

        session.Board[from] = piece;
        session.Board[to] = isEnPassant ? null : targetCaptured;
        piece.SetPosition(from);

        if (isEnPassant)
            session.Board[enPassantVictimPosition] = enPassantVictim;

        if (selfInCheck)
            return MoveValidationResult.Illegal(
                from,
                to,
                "King safety error: The move would leave your king in check."
            );

        return MoveValidationResult.Legal(
            from,
            to,
            piece,
            targetCaptured,
            isEnPassant,
            isCastling,
            rookStartPosition,
            rookTransitPosition,
            castlingRook
        );
    }

    public bool IsLegal(GameSession session, Position from, Position to)
    {
        return Validate(session, from, to).IsLegal;
    }
}
