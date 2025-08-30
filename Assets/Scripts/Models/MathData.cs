using System.Collections.Generic;

public static class MathGameData
{
    // Each gameModeId maps to a LIST of problems
    public static Dictionary<int, List<MathProblemData>> mathGamesList = new Dictionary<int, List<MathProblemData>>();
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
