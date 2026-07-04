using ChessConsoleApp.Core;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;

namespace ChessConsoleApp.Tests.DrawTests;

public static class LackOfMaterialTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= KingVsKing_ShouldBeDrawByInsufficientMaterial();
        allPassed &= KingAndBishopVsKing_ShouldBeDrawByInsufficientMaterial();
        allPassed &= KingAndKnightVsKing_ShouldBeDrawByInsufficientMaterial();
        allPassed &= SameColorBishops_ShouldBeDrawByInsufficientMaterial();

        allPassed &= OppositeColorBishops_ShouldNotBeDrawByInsufficientMaterial();
        allPassed &= KingAndRookVsKing_ShouldNotBeDrawByInsufficientMaterial();
        allPassed &= KingAndPawnVsKing_ShouldNotBeDrawByInsufficientMaterial();
        allPassed &= KingAndQueenVsKing_ShouldNotBeDrawByInsufficientMaterial();
        allPassed &= KingAndTwoKnightsVsKing_ShouldNotBeDrawByInsufficientMaterial();
        allPassed &= StartingPosition_ShouldNotBeDrawByInsufficientMaterial();

        return allPassed;
    }

    public static bool KingVsKing_ShouldBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new King(PieceColor.Black, new Position(7, 4))
        );

        return DrawTestHelpers.ExpectState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "King vs King detected as insufficient material."
        );
    }

    public static bool KingAndBishopVsKing_ShouldBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Bishop(PieceColor.White, new Position(2, 2)),
            new King(PieceColor.Black, new Position(7, 4))
        );

        return DrawTestHelpers.ExpectState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "King + Bishop vs King detected as insufficient material."
        );
    }

    public static bool KingAndKnightVsKing_ShouldBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Knight(PieceColor.White, new Position(2, 2)),
            new King(PieceColor.Black, new Position(7, 4))
        );

        return DrawTestHelpers.ExpectState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "King + Knight vs King detected as insufficient material."
        );
    }

    public static bool SameColorBishops_ShouldBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Bishop(PieceColor.White, new Position(2, 2)),
            new King(PieceColor.Black, new Position(7, 4)),
            new Bishop(PieceColor.Black, new Position(5, 5))
        );

        return DrawTestHelpers.ExpectState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "Same-color bishops detected as insufficient material."
        );
    }

    public static bool OppositeColorBishops_ShouldNotBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Bishop(PieceColor.White, new Position(2, 2)),
            new King(PieceColor.Black, new Position(7, 4)),
            new Bishop(PieceColor.Black, new Position(5, 4))
        );

        return DrawTestHelpers.ExpectNotState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "Opposite-color bishops not detected as insufficient material."
        );
    }

    public static bool KingAndRookVsKing_ShouldNotBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Rook(PieceColor.White, new Position(2, 2)),
            new King(PieceColor.Black, new Position(7, 4))
        );

        return DrawTestHelpers.ExpectNotState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "King + Rook vs King is not insufficient material."
        );
    }

    public static bool KingAndPawnVsKing_ShouldNotBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Pawn(PieceColor.White, new Position(1, 0)),
            new King(PieceColor.Black, new Position(7, 4))
        );

        return DrawTestHelpers.ExpectNotState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "King + Pawn vs King is not insufficient material."
        );
    }

    public static bool KingAndQueenVsKing_ShouldNotBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Queen(PieceColor.White, new Position(2, 2)),
            new King(PieceColor.Black, new Position(7, 4))
        );

        return DrawTestHelpers.ExpectNotState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "King + Queen vs King is not insufficient material."
        );
    }

    public static bool KingAndTwoKnightsVsKing_ShouldNotBeDrawByInsufficientMaterial()
    {
        var engine = DrawTestHelpers.CreateEngineWith(
            new King(PieceColor.White, new Position(0, 4)),
            new Knight(PieceColor.White, new Position(2, 2)),
            new Knight(PieceColor.White, new Position(2, 5)),
            new King(PieceColor.Black, new Position(7, 4))
        );

        return DrawTestHelpers.ExpectNotState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "King + Two Knights vs King is not insufficient material."
        );
    }

    public static bool StartingPosition_ShouldNotBeDrawByInsufficientMaterial()
    {
        var engine = new GameEngine();

        return DrawTestHelpers.ExpectNotState(
            engine.EvaluateGameStateForTesting(),
            GameState.DrawByInsufficientMaterial,
            "Starting position is not insufficient material."
        );
    }
}
