namespace ChessGame.Enums;

public enum GameState
{
    Running,
    Checkmate,
    Stalemate,
    DrawByRepetition,
    DrawByFiftyMoveRule,
    DrawByInsufficientMaterial,
    Resigned,
}
