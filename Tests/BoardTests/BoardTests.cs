using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;

namespace ChessConsoleApp.Tests.BoardTests;

public static class BoardTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= Initialize_ShouldPlaceKingsCorrectly();
        allPassed &= Initialize_ShouldPlaceWhiteBackRankCorrectly();
        allPassed &= Initialize_ShouldPlaceBlackBackRankCorrectly();

        allPassed &= MovePiece_ShouldMovePieceToDestination();
        allPassed &= MovePiece_ShouldClearOriginSquare();
        allPassed &= MovePiece_ShouldUpdatePiecePosition();

        allPassed &= FindKing_ShouldFindWhiteKing();
        allPassed &= FindKing_ShouldFindBlackKing();

        allPassed &= IsSquareEmpty_ShouldReturnTrueForEmptySquare();
        allPassed &= IsEnemyPiece_ShouldDetectEnemyPiece();

        return allPassed;
    }

    public static bool Initialize_ShouldPlaceKingsCorrectly()
    {
        Board board = new Board();
        board.Initialize();

        bool passed =
            board[new Position(0, 4)] is King { Color: PieceColor.White }
            && board[new Position(7, 4)] is King { Color: PieceColor.Black };

        return BoardTestHelpers.PrintResult(passed, "Board initializes both kings correctly.");
    }

    public static bool Initialize_ShouldPlaceWhiteBackRankCorrectly()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        return BoardTestHelpers.PrintResult(
            BoardTestHelpers.HasBackRank(board, 0, PieceColor.White),
            "Board initializes White back rank correctly."
        );
    }

    public static bool Initialize_ShouldPlaceBlackBackRankCorrectly()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        return BoardTestHelpers.PrintResult(
            BoardTestHelpers.HasBackRank(board, 7, PieceColor.Black),
            "Board initializes Black back rank correctly."
        );
    }

    public static bool MovePiece_ShouldMovePieceToDestination()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        Position from = new Position(1, 4); // e2
        Position to = new Position(3, 4); // e4

        Piece? piece = board[from];

        board.MovePiece(from, to);

        bool passed = board[to] == piece;

        return BoardTestHelpers.PrintResult(passed, "MovePiece moves piece to destination.");
    }

    public static bool MovePiece_ShouldClearOriginSquare()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        Position from = new Position(1, 4); // e2
        Position to = new Position(3, 4); // e4

        board.MovePiece(from, to);

        bool passed = board[from] == null;

        return BoardTestHelpers.PrintResult(passed, "MovePiece clears origin square.");
    }

    public static bool MovePiece_ShouldUpdatePiecePosition()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        Position from = new Position(1, 4); // e2
        Position to = new Position(3, 4); // e4

        board.MovePiece(from, to);

        bool passed = board[to]?.Position.Row == to.Row && board[to]?.Position.Column == to.Column;

        return BoardTestHelpers.PrintResult(passed, "MovePiece updates internal piece position.");
    }

    public static bool FindKing_ShouldFindWhiteKing()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        Position kingPosition = board.FindKing(PieceColor.White);

        bool passed = kingPosition.Row == 0 && kingPosition.Column == 4;

        return BoardTestHelpers.PrintResult(passed, "FindKing finds White king.");
    }

    public static bool FindKing_ShouldFindBlackKing()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        Position kingPosition = board.FindKing(PieceColor.Black);

        bool passed = kingPosition.Row == 7 && kingPosition.Column == 4;

        return BoardTestHelpers.PrintResult(passed, "FindKing finds Black king.");
    }

    public static bool IsSquareEmpty_ShouldReturnTrueForEmptySquare()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        bool passed = board.IsSquareEmpty(new Position(3, 3)); // d4

        return BoardTestHelpers.PrintResult(passed, "IsSquareEmpty detects empty square.");
    }

    public static bool IsEnemyPiece_ShouldDetectEnemyPiece()
    {
        Board board = BoardTestHelpers.CreateInitializedBoard();

        bool passed = board.IsEnemyPiece(new Position(6, 4), PieceColor.White); // black pawn on e7

        return BoardTestHelpers.PrintResult(passed, "IsEnemyPiece detects enemy piece.");
    }
}
