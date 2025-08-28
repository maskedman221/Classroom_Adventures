using UnityEngine;
[System.Serializable]
public class Map
{
    public int id;
    public string name;
    public string description;
    public string grade;
    public string image_path;
}

[System.Serializable]
public class Stage
{
    public int id;
    public string name;
    public int order;
    public bool is_completed;
}

[System.Serializable]
public class DataMap
{
    public Map map;
    public Stage[] stages;
}

[System.Serializable]
public class RootResponseMap
{
    public bool success;
    public DataMap data;
}
