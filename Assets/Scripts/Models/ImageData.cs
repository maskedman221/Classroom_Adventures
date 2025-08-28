using System.Collections.Generic;

public static class ImageGameData
{
    // Key = game_mode_instance_id
    public static Dictionary<int, List<ImageProblemData>> imageGames = new Dictionary<int, List<ImageProblemData>>();
}

[System.Serializable]
public class ImageProblemData
{
    public string[] images;
    public int correctAnswer;
    public ImageProblemData(string[] imgs, int answer, int qId)
    {
        images = imgs;
        correctAnswer = answer;
    }
}
