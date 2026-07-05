using ChessConsoleApp.AI;
using ChessConsoleApp.Core.Moves;
using ChessConsoleApp.Core.State;
using ChessConsoleApp.Enums;
using ChessConsoleApp.Models;
using ChessConsoleApp.Models.Pieces;
using ChessConsoleApp.UI;

namespace ChessConsoleApp.Core;

public class GameEngine
{
    private readonly GameSession _session = new();
    private readonly MoveValidator _moveValidator = new();
    private readonly GameMenu _gameMenu = new();
    private readonly MoveExecutor _moveExecutor;
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
        _moveExecutor = new MoveExecutor(_moveValidator);
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

        ConsoleRenderer.RenderBoard(_session.Board, _session.MoveHistory);
        EndGameRenderer.Render(_session);
    }

    public void ShowMainMenu()
    {
        while (true)
        {
            MainMenuOption option = _gameMenu.ShowMainMenu();

            if (option == MainMenuOption.PlayLocal)
            {
                ResetEngineState();
                _isVsAI = false;
                Start();
            }
            else if (option == MainMenuOption.PlayVsComputer)
            {
                ResetEngineState();
                _isVsAI = true;

                _aiDifficulty = _gameMenu.SelectDifficulty();
                _playerColor = _gameMenu.SelectPlayerColor();

                Start();
            }
            else if (option == MainMenuOption.Exit)
            {
                return;
            }
        }
    }

    private void ResetEngineState()
    {
        _session.Initialize();

        PositionSnapshotService.RegisterCurrentPosition(_session);
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
        MoveExecutionResult result = _moveExecutor.Execute(_session, from, to);

        if (!result.Succeeded)
        {
            WriteDiagnostic(result.ErrorMessage ?? "Invalid move.");
            return false;
        }

        if (result.RequiresPromotion && result.MovedPiece != null)
            HandlePromotion(result.PromotionPosition, result.MovedPiece.Color);

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
