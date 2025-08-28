using System.Collections.Generic;

public static class MathGameData
{
    public static Dictionary<int, MathProblemData> mathGames = new Dictionary<int, MathProblemData>();
}

[System.Serializable]
public class MathProblemData
{
    public string question;
    public string answer;

    public MathProblemData(string q, string a)
    {
        question = q;
        answer = a;
    }
}