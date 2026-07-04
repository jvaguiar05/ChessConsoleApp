using ChessGame.Core;
using ChessGame.Enums;
using ChessGame.Models;
using ChessGame.Models.Pieces;

namespace ChessConsoleApp.AI;

public static class ChessAi
{
    private static readonly Random _rng = new();

    public static bool GetBestMove(
        Board board,
        PieceColor aiColor,
        AiDifficulty difficulty,
        Func<Position, Position, bool> isMoveLegalCallback,
        out Position bestFrom,
        out Position bestTo
    )
    {
        var legalMoves = GatherAllLegalMoves(board, aiColor, isMoveLegalCallback);

        if (legalMoves.Count == 0)
        {
            bestFrom = default;
            bestTo = default;
            return false;
        }

        // --- ELO EMULATION VIA PLANNED BLUNDERING ---
        if (difficulty == AiDifficulty.Beginner && _rng.NextDouble() < 0.50)
        {
            var randomMove = legalMoves[_rng.Next(legalMoves.Count)];
            bestFrom = randomMove.From;
            bestTo = randomMove.To;
            return true;
        }

        // Map difficulty tiers cleanly to tree search depths
        int targetDepth = difficulty switch
        {
            AiDifficulty.Beginner => 1,
            AiDifficulty.Intermediate => 3,
            AiDifficulty.Advanced => 4,
            AiDifficulty.Master => 5,
            AiDifficulty.Grandmaster => 6, // Warning: Depths > 6 might take a few seconds in basic C# array models
            _ => 3,
        };

        int bestScore = aiColor == PieceColor.White ? int.MinValue : int.MaxValue;
        var topTierMoves = new List<(Position From, Position To)>();

        int alpha = int.MinValue;
        int beta = int.MaxValue;

        foreach (var move in legalMoves)
        {
            // Simulate the move locally
            Piece? originalTarget = board[move.To];
            Piece? movingPiece = board[move.From];

            board[move.To] = movingPiece;
            board[move.From] = null;
            movingPiece?.SetPosition(move.To);

            // Invoke Minimax depth tree
            int score = Minimax(
                board,
                targetDepth - 1,
                alpha,
                beta,
                aiColor == PieceColor.White,
                isMoveLegalCallback
            );

            // Undo the move
            board[move.From] = movingPiece;
            board[move.To] = originalTarget;
            movingPiece?.SetPosition(move.From);

            if (aiColor == PieceColor.White)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    topTierMoves.Clear();
                    topTierMoves.Add(move);
                }
                else if (score == bestScore)
                    topTierMoves.Add(move);

                alpha = Math.Max(alpha, bestScore);
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    topTierMoves.Clear();
                    topTierMoves.Add(move);
                }
                else if (score == bestScore)
                    topTierMoves.Add(move);

                beta = Math.Min(beta, bestScore);
            }
        }

        var selected = topTierMoves[_rng.Next(topTierMoves.Count)];
        bestFrom = selected.From;
        bestTo = selected.To;
        return true;
    }

    private static int Minimax(
        Board board,
        int depth,
        int alpha,
        int beta,
        bool isMaximizingPlayer,
        Func<Position, Position, bool> isMoveLegalCallback
    )
    {
        if (depth == 0)
            return BoardEvaluator.Evaluate(board);

        PieceColor activeColor = isMaximizingPlayer ? PieceColor.White : PieceColor.Black;
        var legalMoves = GatherAllLegalMoves(board, activeColor, isMoveLegalCallback);

        // If no legal moves, it's checkmate or stalemate
        if (legalMoves.Count == 0)
            return isMaximizingPlayer ? -15000 : 15000;

        if (isMaximizingPlayer)
        {
            int maxScore = int.MinValue;
            foreach (var move in legalMoves)
            {
                Piece? target = board[move.To];
                Piece? piece = board[move.From];

                board[move.To] = piece;
                board[move.From] = null;
                piece?.SetPosition(move.To);

                int score = Minimax(board, depth - 1, alpha, beta, false, isMoveLegalCallback);

                board[move.From] = piece;
                board[move.To] = target;
                piece?.SetPosition(move.From);

                maxScore = Math.Max(maxScore, score);
                alpha = Math.Max(alpha, score);
                if (beta <= alpha)
                    break; // Beta cut-off
            }
            return maxScore;
        }
        else
        {
            int minScore = int.MaxValue;
            foreach (var move in legalMoves)
            {
                Piece? target = board[move.To];
                Piece? piece = board[move.From];

                board[move.To] = piece;
                board[move.From] = null;
                piece?.SetPosition(move.To);

                int score = Minimax(board, depth - 1, alpha, beta, true, isMoveLegalCallback);

                board[move.From] = piece;
                board[move.To] = target;
                piece?.SetPosition(move.From);

                minScore = Math.Min(minScore, score);
                beta = Math.Min(beta, score);
                if (beta <= alpha)
                    break; // Alpha cut-off
            }
            return minScore;
        }
    }

    private static List<(Position From, Position To)> GatherAllLegalMoves(
        Board board,
        PieceColor color,
        Func<Position, Position, bool> isMoveLegalCallback
    )
    {
        var list = new List<(Position From, Position To)>();
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                Position from = new Position(r, c);
                Piece? p = board[from];
                if (p == null || p.Color != color)
                    continue;

                for (int targetR = 0; targetR < 8; targetR++)
                {
                    for (int targetC = 0; targetC < 8; targetC++)
                    {
                        Position to = new Position(targetR, targetC);
                        if (isMoveLegalCallback(from, to))
                        {
                            list.Add((from, to));
                        }
                    }
                }
            }
        }
        return list;
    }
}
