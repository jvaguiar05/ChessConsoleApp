using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;

namespace ChessConsoleApp.Core;

public sealed class GameSession
{
    public Board Board { get; } = new();
    public List<Move> MoveHistory { get; } = new();
    public Dictionary<string, int> PositionHistory { get; } = new();

    public PieceColor CurrentTurn { get; set; } = PieceColor.White;
    public GameState State { get; set; } = GameState.Running;
    public int HalfMoveClock { get; set; }

    public Move? LastMove => MoveHistory.Count > 0 ? MoveHistory[^1] : null;

    public void Initialize()
    {
        Board.Initialize();
        MoveHistory.Clear();
        PositionHistory.Clear();

        CurrentTurn = PieceColor.White;
        State = GameState.Running;
        HalfMoveClock = 0;
    }

    public void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }
}
