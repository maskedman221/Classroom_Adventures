 [System.Serializable]
public class UpdateProgressResponse
{
    public bool success;
    public string message;
    public ProgressData data;
}

[System.Serializable]
public class ProgressData
{
    public int child_id;
    public int current_stage_id;
}