using ChessConsoleApp.AI;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;
using ChessConsoleApp.UI;

namespace ChessConsoleApp.Core;

public class GameEngine
{
    private readonly ChessGameState _gameState = new();
    private bool _isVsAI = false;
    private PieceColor _playerColor = PieceColor.White;
    private AiDifficulty _aiDifficulty = AiDifficulty.Intermediate;

#if DEBUG
    private bool _suppressDiagnostics;
    private char? _promotionChoiceForTesting;
#endif

    public GameEngine()
    {
        _gameState.Initialize();

        string initialSnapshot = GeneratePositionSnapshot();
        _gameState.PositionHistory[initialSnapshot] = 1;
    }

    /// <summary>
    /// Coordinates the main state-machine driven game loop.
    /// </summary>
    public void Start()
    {
        while (_gameState.State == GameState.Running)
        {
            ConsoleRenderer.RenderBoard(_gameState.Board, _gameState.MoveHistory);
            EvaluateGameState();

            if (_gameState.State != GameState.Running)
                continue;

            Move? lastMove = _gameState.MoveHistory.Count > 0 ? _gameState.MoveHistory[^1] : null;

            if (_gameState.Board.IsColorInCheck(_gameState.CurrentTurn, lastMove))
                Console.WriteLine($"[!] ALERT: {_gameState.CurrentTurn} is currently in CHECK!");

            if (_isVsAI && _gameState.CurrentTurn != _playerColor)
            {
                ExecuteAiTurn();
                continue; // Skips the human Console.ReadLine() and loops back to redraw the AI's move!
            }

            Console.WriteLine(
                $"{_gameState.CurrentTurn}'s turn. Enter move (e.g., 'e2 e4') or 'quit':"
            );
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (input == "quit")
            {
                _gameState.State = GameState.Resigned;
                continue;
            }

            ProcessMoveInput(input);
        }
        // --- THE END PRESENTATION LAYER ---
        // Render the definitive final frame and break down conclusive outcomes via state layout mapping
        ConsoleRenderer.RenderBoard(_gameState.Board, _gameState.MoveHistory);

        switch (_gameState.State)
        {
            case GameState.Checkmate:
                PieceColor winner =
                    _gameState.CurrentTurn == PieceColor.White
                        ? PieceColor.Black
                        : PieceColor.White;
                Console.WriteLine($"\n[!!!] CHECKMATE! {winner} wins the game!");
                break;
            case GameState.Stalemate:
                Console.WriteLine(
                    "\n[---] STALEMATE! The active player has no legal moves. The game is a draw."
                );
                break;
            case GameState.Resigned:
                PieceColor victor =
                    _gameState.CurrentTurn == PieceColor.White
                        ? PieceColor.Black
                        : PieceColor.White;
                Console.WriteLine(
                    $"\n[x] RESIGNATION! {_gameState.CurrentTurn} threw in the towel. {victor} wins the match!"
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

    public void ShowMainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("       CONSOLE CHESS CHAMPIONSHIP       ");
            Console.WriteLine("========================================");
            Console.WriteLine(" 1. Play Local Pass-and-Play (2P)");
            Console.WriteLine(" 2. Play vs Computer (AI)");
            Console.WriteLine(" 3. Exit Game");
            Console.Write("\nSelect an option: ");

            string choice = Console.ReadLine() ?? "";
            if (choice == "1")
            {
                ResetEngineState();
                _isVsAI = false;
                Start();
                // Removed break; -> control returns here when game ends, looping back to menu!
            }
            else if (choice == "2")
            {
                ResetEngineState();
                _isVsAI = true;
                SelectDifficultyMenu();
            }
            else if (choice == "3")
            {
                return; // Expressly exits the application loop entirely
            }
        }
    }

    private void SelectDifficultyMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("       SELECT COMPUTER DIFFICULTY       ");
            Console.WriteLine("========================================");
            Console.WriteLine(" 1. Beginner     (600 Elo)");
            Console.WriteLine(" 2. Intermediate (1200 Elo)");
            Console.WriteLine(" 3. Advanced     (1600 Elo)");
            Console.WriteLine(" 4. Master       (2000 Elo)");
            Console.WriteLine(" 5. Grandmaster  (2500+ Elo)");
            Console.Write("\nSelect your opponent: ");

            string choice = Console.ReadLine() ?? "";
            if (choice == "1")
            {
                _aiDifficulty = AiDifficulty.Beginner;
                break;
            }
            if (choice == "2")
            {
                _aiDifficulty = AiDifficulty.Intermediate;
                break;
            }
            if (choice == "3")
            {
                _aiDifficulty = AiDifficulty.Advanced;
                break;
            }
            if (choice == "4")
            {
                _aiDifficulty = AiDifficulty.Master;
                break;
            }
            if (choice == "5")
            {
                _aiDifficulty = AiDifficulty.Grandmaster;
                break;
            }
        }

        // Advance to the color selection screen
        SelectColorMenu();
    }

    private void ResetEngineState()
    {
        _gameState.Initialize();

        // Log the fresh start configuration into the repetition dictionary
        string initialSnapshot = GeneratePositionSnapshot();
        _gameState.PositionHistory[initialSnapshot] = 1;
    }

    private void SelectColorMenu()
    {
        Console.Clear();
        Console.WriteLine("=== SELECT YOUR COLOR ===");
        Console.WriteLine(" 1. White (Moves First)");
        Console.WriteLine(" 2. Black");
        Console.Write("\nSelect an option: ");

        string choice = Console.ReadLine() ?? "";
        _playerColor = choice == "2" ? PieceColor.Black : PieceColor.White;
        Start();
    }

    private void ProcessMoveInput(string input)
    {
        if (!InputParser.TryParseMove(input, out Position from, out Position to))
        {
            WriteDiagnostic("Invalid move format or coordinates. Use syntax like 'e2 e4'.");
#if DEBUG
            if (!_suppressDiagnostics)
#endif
                Console.ReadKey();
            return;
        }

        if (ExecuteMove(from, to))
        {
            _gameState.SwitchTurn();

            RegisterCurrentPosition();
        }
        else
        {
#if DEBUG
            if (!_suppressDiagnostics)
#endif
                Console.ReadKey();
        }
    }

    private bool ExecuteMove(Position from, Position to)
    {
        Piece? piece = _gameState.Board[from];

        if (piece == null)
        {
            WriteDiagnostic("Selection error: No piece exists at the specified source coordinate.");
            return false;
        }

        if (piece.Color != _gameState.CurrentTurn)
        {
            WriteDiagnostic($"Turn error: It is currently {_gameState.CurrentTurn}'s turn.");
            return false;
        }

        if (
            !EvaluateMoveLegality(
                from,
                to,
                out bool isEnPassant,
                out Piece? targetCaptured,
                out bool isCastling,
                out Position rookStartPos,
                out Position rookTransitPos,
                out Piece? castlingRook
            )
        )
        {
            WriteDiagnostic($"Movement error: Invalid rule trajectory for {piece.GetType().Name}.");
            return false;
        }

        _gameState.MoveHistory.Add(new Move(from, to, piece, targetCaptured));
        _gameState.Board.MovePiece(from, to);

        if (piece is Pawn || targetCaptured != null)
            _gameState.HalfMoveClock = 0;
        else
            _gameState.HalfMoveClock++;

        if (isCastling && castlingRook != null)
            _gameState.Board.MovePiece(rookStartPos, rookTransitPos);

        if (piece is Pawn && (to.Row == 0 || to.Row == 7))
            HandlePromotion(to, piece.Color);

        return true;
    }

    private void ExecuteAiTurn()
    {
        Console.WriteLine($"\n[CPU] Computer ({_gameState.CurrentTurn}) is thinking...");
        System.Threading.Thread.Sleep(800);

        // Call out to the AI layer using clean out parameters
        if (
            ChessAi.GetBestMove(
                _gameState.Board,
                _gameState.CurrentTurn,
                _aiDifficulty,
                IsMoveLegal,
                out Position from,
                out Position to
            )
        )
        {
            string inputFormat = $"{from} {to}";
            ProcessMoveInput(inputFormat);
        }
        else
            // Fallback safety barrier in case the AI has zero legal moves (Checkmate/Stalemate)
            WriteDiagnostic("[CPU] No legal moves available for AI.");
    }

    private bool IsMoveLegal(Position from, Position to) =>
        EvaluateMoveLegality(from, to, out _, out _, out _, out _, out _, out _);

    private bool EvaluateMoveLegality(
        Position from,
        Position to,
        out bool isEnPassant,
        out Piece? targetCaptured,
        out bool isCastling,
        out Position rookStartPos,
        out Position rookTransitPos,
        out Piece? castlingRook
    )
    {
        targetCaptured = _gameState.Board[to];
        isEnPassant = false;
        isCastling = false;
        rookStartPos = default;
        rookTransitPos = default;
        castlingRook = null;

        Piece? piece = _gameState.Board[from];
        if (piece == null || piece.Color != _gameState.CurrentTurn)
            return false;

        Move? lastMove = _gameState.MoveHistory.Count > 0 ? _gameState.MoveHistory[^1] : null;
        if (!piece.IsValidMove(_gameState.Board, to, lastMove))
            return false;

        isEnPassant = piece is Pawn && from.Column != to.Column && targetCaptured == null;
        Position enPassantVictimPos = new Position(from.Row, to.Column);
        Piece? enPassantVictim = isEnPassant ? _gameState.Board[enPassantVictimPos] : null;
        if (isEnPassant)
            targetCaptured = enPassantVictim;

        isCastling = piece is King && Math.Abs(from.Column - to.Column) == 2;

        if (isCastling)
        {
            if (
                piece.HasMoved(_gameState.MoveHistory)
                || _gameState.Board.IsColorInCheck(_gameState.CurrentTurn, lastMove)
            )
                return false;

            int rookStartCol = to.Column == 6 ? 7 : 0;
            int rookTransitCol = to.Column == 6 ? 5 : 3;
            rookStartPos = new Position(from.Row, rookStartCol);
            rookTransitPos = new Position(from.Row, rookTransitCol);
            castlingRook = _gameState.Board[rookStartPos];

            if (castlingRook == null || castlingRook.HasMoved(_gameState.MoveHistory))
                return false;

            int step = to.Column == 6 ? 1 : -1;
            Position transitPos = new Position(from.Row, from.Column + step);
            _gameState.Board[transitPos] = piece;
            _gameState.Board[from] = null;
            piece.SetPosition(transitPos);

            bool transitInCheck = _gameState.Board.IsColorInCheck(_gameState.CurrentTurn, lastMove);

            _gameState.Board[from] = piece;
            _gameState.Board[transitPos] = null;
            piece.SetPosition(from);

            if (transitInCheck)
                return false;
        }

        _gameState.Board[to] = piece;
        _gameState.Board[from] = null;
        piece.SetPosition(to);
        if (isEnPassant)
            _gameState.Board[enPassantVictimPos] = null;

        bool selfInCheck = _gameState.Board.IsColorInCheck(_gameState.CurrentTurn, lastMove);

        _gameState.Board[from] = piece;
        _gameState.Board[to] = isEnPassant ? null : targetCaptured;
        piece.SetPosition(from);
        if (isEnPassant)
            _gameState.Board[enPassantVictimPos] = enPassantVictim;

        return !selfInCheck;
    }

    private string GeneratePositionSnapshot()
    {
        var sb = new System.Text.StringBuilder();

        // 1. Map piece positions across the entire grid
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                Piece? p = _gameState.Board[new Position(r, c)];
                if (p == null)
                    sb.Append('.');
                else
                {
                    // White pieces are UPPERCASE, Black pieces are lowercase
                    char symbol = p is Knight ? 'N' : p.GetType().Name[0];
                    sb.Append(
                        p.Color == PieceColor.White ? char.ToUpper(symbol) : char.ToLower(symbol)
                    );
                }
            }
        }

        // 2. Append active turn modifier to separate positions with different players moving next
        sb.Append($"|{_gameState.CurrentTurn}");

        return sb.ToString();
    }

    private void HandlePromotion(Position position, PieceColor color)
    {
#if DEBUG
        if (_promotionChoiceForTesting.HasValue)
        {
            _gameState.Board[position] = CreatePromotionPiece(
                _promotionChoiceForTesting.Value,
                color,
                position
            );

            return;
        }
#endif

        Console.WriteLine(
            $"\nPawn Promotion! Choose a piece for {color} (Q = Queen, R = Rook, B = Bishop, N = Knight):"
        );

        while (true)
        {
            string choice = Console.ReadLine()?.Trim().ToUpper() ?? "Q";

            _gameState.Board[position] = CreatePromotionPiece(choice[0], color, position);

            break;
        }
    }

    private void RegisterCurrentPosition()
    {
        string snapshot = GeneratePositionSnapshot();

        if (_gameState.PositionHistory.TryGetValue(snapshot, out int count))
            _gameState.PositionHistory[snapshot] = count + 1;
        else
            _gameState.PositionHistory[snapshot] = 1;
    }

    private GameState EvaluateGameState()
    {
        Move? lastMove = _gameState.MoveHistory.Count > 0 ? _gameState.MoveHistory[^1] : null;

        if (_gameState.HalfMoveClock >= 100)
            return _gameState.State = GameState.DrawByFiftyMoveRule;

        string currentSnapshot = GeneratePositionSnapshot();
        if (
            _gameState.PositionHistory.TryGetValue(currentSnapshot, out int repCount)
            && repCount >= 3
        )
            return _gameState.State = GameState.DrawByRepetition;

        if (_gameState.Board.HasInsufficientMaterial())
            return _gameState.State = GameState.DrawByInsufficientMaterial;

        bool inCheck = _gameState.Board.IsColorInCheck(_gameState.CurrentTurn, lastMove);
        bool hasMoves = _gameState.Board.HasAnyLegalMoves(
            _gameState.CurrentTurn,
            lastMove,
            IsMoveLegal
        );

        if (!hasMoves)
            return _gameState.State = inCheck ? GameState.Checkmate : GameState.Stalemate;

        return _gameState.State;
    }

    private static Piece CreatePromotionPiece(char choice, PieceColor color, Position position)
    {
        return char.ToUpper(choice) switch
        {
            'R' => new Rook(color, position),
            'B' => new Bishop(color, position),
            'N' => new Knight(color, position),
            _ => new Queen(color, position),
        };
    }

    private void WriteDiagnostic(string message)
    {
#if DEBUG
        if (_suppressDiagnostics)
            return;
#endif

        Console.WriteLine(message);
    }

#if DEBUG
    public GameState TestMoveInput(string input)
    {
        ProcessMoveInput(input.Trim().ToLower());
        return EvaluateGameState();
    }

    public void SetHalfMoveClockForTesting(int value) => _gameState.HalfMoveClock = value;

    public Board BoardForTesting => _gameState.Board;

    public GameState EvaluateGameStateForTesting() => EvaluateGameState();

    public void SetCurrentTurnForTesting(PieceColor color) => _gameState.CurrentTurn = color;

    public void SuppressDiagnosticsForTesting() => _suppressDiagnostics = true;

    public void SetPromotionChoiceForTesting(char choice) =>
        _promotionChoiceForTesting = char.ToUpper(choice);

#endif
}
