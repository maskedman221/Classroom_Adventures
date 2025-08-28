[System.Serializable]
public class RootResponse
{
    public Child[] data; // match the JSON property name "data"
}

[System.Serializable]
public class Child
{
    public int id;
    public string name;
    public int? age; // nullable int
    public int grade_id;
    public string grade;
    public int user_id;
    public string created_at;
    public string updated_at;
    public int current_stage_id;
}