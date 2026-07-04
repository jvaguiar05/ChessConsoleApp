using ChessGame.Models;

namespace ChessConsoleApp.Tests.NotationTests;

public static class NotationTests
{
    public static bool RunAll()
    {
        bool allPassed = true;

        allPassed &= AlgebraicNotation_ShouldConvertA1ToPosition();
        allPassed &= AlgebraicNotation_ShouldConvertH8ToPosition();
        allPassed &= AlgebraicNotation_ShouldConvertE4ToPosition();

        allPassed &= Position_ShouldConvertToA1();
        allPassed &= Position_ShouldConvertToH8();
        allPassed &= Position_ShouldConvertToE4();

        return allPassed;
    }

    public static bool AlgebraicNotation_ShouldConvertA1ToPosition()
    {
        Position position = Position.FromAlgebraic("a1");

        if (position.Row == 0 && position.Column == 0)
        {
            Console.WriteLine(" [PASS] 'a1' converted correctly.");
            return true;
        }

        Console.WriteLine(" [FAIL] 'a1' converted incorrectly.");
        return false;
    }

    public static bool AlgebraicNotation_ShouldConvertH8ToPosition()
    {
        Position position = Position.FromAlgebraic("h8");

        if (position.Row == 7 && position.Column == 7)
        {
            Console.WriteLine(" [PASS] 'h8' converted correctly.");
            return true;
        }

        Console.WriteLine(" [FAIL] 'h8' converted incorrectly.");
        return false;
    }

    public static bool AlgebraicNotation_ShouldConvertE4ToPosition()
    {
        Position position = Position.FromAlgebraic("e4");

        if (position.Row == 3 && position.Column == 4)
        {
            Console.WriteLine(" [PASS] 'e4' converted correctly.");
            return true;
        }

        Console.WriteLine(" [FAIL] 'e4' converted incorrectly.");
        return false;
    }

    public static bool Position_ShouldConvertToA1()
    {
        Position position = new Position(0, 0);

        if (position.ToString() == "a1")
        {
            Console.WriteLine(" [PASS] Position (0,0) converted to 'a1'.");
            return true;
        }

        Console.WriteLine(" [FAIL] Position (0,0) converted incorrectly.");
        return false;
    }

    public static bool Position_ShouldConvertToH8()
    {
        Position position = new Position(7, 7);

        if (position.ToString() == "h8")
        {
            Console.WriteLine(" [PASS] Position (7,7) converted to 'h8'.");
            return true;
        }

        Console.WriteLine(" [FAIL] Position (7,7) converted incorrectly.");
        return false;
    }

    public static bool Position_ShouldConvertToE4()
    {
        Position position = new Position(3, 4);

        if (position.ToString() == "e4")
        {
            Console.WriteLine(" [PASS] Position (3,4) converted to 'e4'.");
            return true;
        }

        Console.WriteLine(" [FAIL] Position (3,4) converted incorrectly.");
        return false;
    }
}
