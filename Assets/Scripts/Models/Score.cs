using UnityEngine;

public static class Score
{
    private static float starsScore = 1 / 3;
    public static int countGames;
    public static int gotIt = 0;
    public static int GetScore()
    {
    if (countGames == 0) return 0; // avoid division by zero
    float fraction = (float)gotIt / countGames;
    if (fraction < 1f / 3f) return 0;
    else if (fraction > 1f / 3f && fraction< 2f / 3f) return 1;
    else if (fraction < 1f) return 2;
    else return 3;
    }
}