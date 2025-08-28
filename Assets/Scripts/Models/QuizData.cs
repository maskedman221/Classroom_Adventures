using System.Collections.Generic;

public static class QuizGameData
{
    public static Dictionary<int, List<QuizQuestionData>> quizGames = new Dictionary<int, List<QuizQuestionData>>();
    public static void AddQuestion(int gameModeId, QuizQuestionData question)
    {
        if (!quizGames.ContainsKey(gameModeId))
        {
            quizGames[gameModeId] = new List<QuizQuestionData>();
        }
        quizGames[gameModeId].Add(question);
    }
}

[System.Serializable]
public class QuizQuestionData
{
    public string question;
    public string[] choices;
    public int correctOption;

    public QuizQuestionData(string question, string[] choices, int correctOption)
    {
        this.question = question;
        this.choices = choices;
        this.correctOption = correctOption;
    }
}
