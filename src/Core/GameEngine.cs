using ChessConsoleApp.AI;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;
using ChessConsoleApp.UI;

namespace ChessConsoleApp.Core;

public class GameEngine
{
    private readonly GameSession _session = new();
    private readonly MoveValidator _moveValidator = new();
    private readonly GameStateEvaluator _gameStateEvaluator;
    private bool _isVsAI = false;
    private PieceColor _playerColor = PieceColor.White;
    private AiDifficulty _aiDifficulty = AiDifficulty.Intermediate;

#if DEBUG
    private bool _suppressDiagnostics;
    private char? _promotionChoiceForTesting;
#endif

    public GameEngine()
    {
        _gameStateEvaluator = new GameStateEvaluator(IsMoveLegal);

        _session.Initialize();

        PositionSnapshotService.RegisterCurrentPosition(_session);
    }

    /// <summary>
    /// Coordinates the main state-machine driven game loop.
    /// </summary>
    public void Start()
    {
        while (_session.State == GameState.Running)
        {
            ConsoleRenderer.RenderBoard(_session.Board, _session.MoveHistory);
            _gameStateEvaluator.Evaluate(_session);

            if (_session.State != GameState.Running)
                continue;

            Move? lastMove = _session.LastMove;

            if (_session.Board.IsColorInCheck(_session.CurrentTurn, lastMove))
                Console.WriteLine($"[!] ALERT: {_session.CurrentTurn} is currently in CHECK!");

            if (_isVsAI && _session.CurrentTurn != _playerColor)
            {
                ExecuteAiTurn();
                continue; // Skips the human Console.ReadLine() and loops back to redraw the AI's move!
            }

            Console.WriteLine(
                $"{_session.CurrentTurn}'s turn. Enter move (e.g., 'e2 e4') or 'quit':"
            );
            string input = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (input == "quit")
            {
                _session.State = GameState.Resigned;
                continue;
            }

            ProcessMoveInput(input);
        }
        // --- THE END PRESENTATION LAYER ---
        // Render the definitive final frame and break down conclusive outcomes via state layout mapping
        ConsoleRenderer.RenderBoard(_session.Board, _session.MoveHistory);

        switch (_session.State)
        {
            case GameState.Checkmate:
                PieceColor winner =
                    _session.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                Console.WriteLine($"\n[!!!] CHECKMATE! {winner} wins the game!");
                break;
            case GameState.Stalemate:
                Console.WriteLine(
                    "\n[---] STALEMATE! The active player has no legal moves. The game is a draw."
                );
                break;
            case GameState.Resigned:
                PieceColor victor =
                    _session.CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                Console.WriteLine(
                    $"\n[x] RESIGNATION! {_session.CurrentTurn} threw in the towel. {victor} wins the match!"
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
        _session.Initialize();

        PositionSnapshotService.RegisterCurrentPosition(_session);
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
            _session.SwitchTurn();

            PositionSnapshotService.RegisterCurrentPosition(_session);
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
        MoveValidationResult validation = _moveValidator.Validate(_session, from, to);

        if (!validation.IsLegal)
        {
            WriteDiagnostic(validation.ErrorMessage ?? "Invalid move.");
            return false;
        }

        Piece piece = validation.MovingPiece!;

        _session.MoveHistory.Add(new Move(from, to, piece, validation.CapturedPiece));
        _session.Board.MovePiece(from, to);

        if (piece is Pawn || validation.CapturedPiece != null)
            _session.HalfMoveClock = 0;
        else
            _session.HalfMoveClock++;

        if (validation.IsCastling && validation.CastlingRook != null)
            _session.Board.MovePiece(validation.RookStartPosition, validation.RookTransitPosition);

        if (piece is Pawn && (to.Row == 0 || to.Row == 7))
            HandlePromotion(to, piece.Color);

        return true;
    }

    private void ExecuteAiTurn()
    {
        Console.WriteLine($"\n[CPU] Computer ({_session.CurrentTurn}) is thinking...");
        System.Threading.Thread.Sleep(800);

        // Call out to the AI layer using clean out parameters
        if (
            ChessAi.GetBestMove(
                _session.Board,
                _session.CurrentTurn,
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
        _moveValidator.IsLegal(_session, from, to);

    private void HandlePromotion(Position position, PieceColor color)
    {
#if DEBUG
        if (_promotionChoiceForTesting.HasValue)
        {
            _session.Board[position] = CreatePromotionPiece(
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

            _session.Board[position] = CreatePromotionPiece(choice[0], color, position);

            break;
        }
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
        return _gameStateEvaluator.Evaluate(_session);
    }

    public GameState EvaluateGameStateForTesting() => _gameStateEvaluator.Evaluate(_session);

    public void SetHalfMoveClockForTesting(int value) => _session.HalfMoveClock = value;

    public Board BoardForTesting => _session.Board;

    public void SetCurrentTurnForTesting(PieceColor color) => _session.CurrentTurn = color;

    public void SuppressDiagnosticsForTesting() => _suppressDiagnostics = true;

    public void SetPromotionChoiceForTesting(char choice) =>
        _promotionChoiceForTesting = char.ToUpper(choice);

#endif
}
