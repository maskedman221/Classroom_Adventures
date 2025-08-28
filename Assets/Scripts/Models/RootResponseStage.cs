using System;
using UnityEngine;

[Serializable]
public class Lesson
{
    public int id;
    public int stage_id;
    public int? subject_id;
    public string title;
    public string description;
    public int order;
    public bool is_active;
    public string created_at;
    public string updated_at;
    public string image_path;
    public string video_path;

    public bool HasImage => !string.IsNullOrEmpty(image_path);
    public bool HasVideo => !string.IsNullOrEmpty(video_path);
}

[Serializable]
public class PhotoEntry
{
    public int id;
    public int game_mode_instance_id;
    public string[] correct_images;
    public string[] wrong_images;
    public int answer;
    public string created_at;
    public string updated_at;

    public bool HasCorrectImages => correct_images != null && correct_images.Length > 0;
    public bool HasWrongImages => wrong_images != null && wrong_images.Length > 0;
}
[Serializable]
public class MathProblem
{
    public int id;
    public int game_mode_instance_id;
    public string question;
    public string answer;
    public string created_at;
    public string updated_at;
}
[Serializable]
public class QuizQuestion
{
    public int id;
    public int game_mode_instance_id;
    public string question;
    public string choices; // stored as JSON string in your DB
    public int correct_option;
    public string created_at;
    public string updated_at;

    // Helper to parse choices into array
    public string[] GetChoices()
    {
        if (string.IsNullOrEmpty(choices)) return new string[0];
        return JsonHelper.FromJson<string>(choices);
    }
}
public enum GameModeType
{
    EMPTY,
    PHOTO,
    QUIZ,
    MATH
}

[Serializable]
public class GameMode
{
    public int id;
    public string type;
    public string name;
    public string description;
    public string config;
    public PhotoEntry[] photo_entries;
    public QuizQuestion[] quiz_questions;
    public MathProblem[] math_problems;

    public bool HasPhotoEntries => photo_entries != null && photo_entries.Length > 0;
    public bool HasQuizQuestions => quiz_questions != null && quiz_questions.Length > 0;
    public bool HasMathProblems => math_problems != null && math_problems.Length > 0;

    public GameModeType GetActiveModeType()
    {
        if (HasPhotoEntries) return GameModeType.PHOTO;
        if (HasQuizQuestions) return GameModeType.QUIZ;
        if (HasMathProblems) return GameModeType.MATH;
        return GameModeType.EMPTY;
    }
}

[Serializable]
public class RootResponseStage
{
    public string stage;
    public Lesson[] lessons;
    public GameMode[] game_modes;
}
