using UnityEngine;

public enum MatchResult
{
    None,
    HamurWins,
    MarshmallowWins,
    Draw
}

public static class GameResult
{
    public static MatchResult Result { get; private set; } = MatchResult.None;

    public static void HamurWon()
    {
        Result = MatchResult.HamurWins;
    }

    public static void MarshmallowWon()
    {
        Result = MatchResult.MarshmallowWins;
    }

    public static void Draw()
    {
        Result = MatchResult.Draw;
    }

    // İkisini aynı anda kontrol etmek istersen
    public static void SetResult(bool hamurWon, bool marshmallowWon)
    {
        if (hamurWon && marshmallowWon)
        {
            Result = MatchResult.Draw;
        }
        else if (hamurWon)
        {
            Result = MatchResult.HamurWins;
        }
        else if (marshmallowWon)
        {
            Result = MatchResult.MarshmallowWins;
        }
    }

    public static void ResetResult()
    {
        Result = MatchResult.None;
    }
}