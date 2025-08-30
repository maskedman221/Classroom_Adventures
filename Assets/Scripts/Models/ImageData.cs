using System.Collections.Generic;

public static class ImageGameData
{
    public static Dictionary<int, List<ImageProblemData>> imageGames = new Dictionary<int, List<ImageProblemData>>();

    public static void AddProblem(int gameModeId, ImageProblemData problem)
    {
        if (!imageGames.ContainsKey(gameModeId))
        {
            imageGames[gameModeId] = new List<ImageProblemData>();
        }
        imageGames[gameModeId].Add(problem);
    }
}

[System.Serializable]
public class ImageProblemData
{
    public string question;      
    public string[] images;
    public int correctAnswer;

    public ImageProblemData(string q, string[] imgs, int answer)
    {
        question = q;
        images = imgs;
        correctAnswer = answer;
    }
}
