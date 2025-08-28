using UnityEngine;
using System;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    public event EventHandler OnStageLoaded;
    ApiGetLoader api = new ApiGetLoader();
    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        Debug.Log($"MapData.idClicked{MapData.idClicked}");
        var response = await LoadingManager.Instance.RunWithLoading(() => api.GetStage(MapData.idClicked));
        foreach (var lesson in response.lessons)
        {
            LessonsData.titles.Add(lesson.title);
            LessonsData.descriptions.Add(lesson.description);
            LessonsData.image_paths.Add(lesson.title);
            LessonsData.video_paths.Add(lesson.title);
        }
        foreach (var gm in response.game_modes)
        {
            GameModeType type = gm.GetActiveModeType();
            if (type == GameModeType.PHOTO)
            {
                MapData.gamemode = "PHOTO";
                for (int i = 0; i < response.game_modes[0].photo_entries.Length; i++)
                {
                    string[] array1 = response.game_modes[0].photo_entries[i].correct_images;
                    string[] array2 = response.game_modes[0].photo_entries[i].wrong_images;
                    // Combine arrays
                    string[] combinedArray = new string[array1.Length + array2.Length];
                    array1.CopyTo(combinedArray, 0);
                    array2.CopyTo(combinedArray, array1.Length);
                    string[] shuffledArray = ShuffleArray(combinedArray);
                    ImageGameData.answers.Add(response.game_modes[0].photo_entries[i].answer);
                    ImageGameData.AllImages.Add(shuffledArray);
                }
            }
            if (type == GameModeType.MATH)
            {
                MapData.gamemode = "MATH";
                for (int i = 0; i < response.game_modes[0].math_problems.Length; i++)
                {
                    MathGameData.mathGames[response.game_modes[0].math_problems[i].id] =
                    new MathProblemData(response.game_modes[0].math_problems[i].question, response.game_modes[0].math_problems[i].answer);
                }
            }
            if (type == GameModeType.QUIZ)
            {
                MapData.gamemode = "QUIZ";
                for (int i = 0; i < response.game_modes[0].quiz_questions.Length; i++)
                {
                    Debug.Log(response.game_modes[0].quiz_questions.Length);
                    QuizGameData.AddQuestion(response.game_modes[0].id, new QuizQuestionData(
                        response.game_modes[0].quiz_questions[i].question,
                        response.game_modes[0].quiz_questions[i].GetChoices(),
                        response.game_modes[0].quiz_questions[i].correct_option
                    ));
                }
            }
        }
        OnStageLoaded?.Invoke(this, EventArgs.Empty);
    }

    private string[] ShuffleArray(string[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            string temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }
}
