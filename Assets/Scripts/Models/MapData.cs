using System.Collections.Generic;

[System.Serializable]
public class MapData
{
    public string playerName;
    public string playerGrade;
    public int[] current_stage_id = new int[3];
    public int childId;
    public int idClicked;
    public int order;
    public string gamemode;
    public List<Stage> stages = new List<Stage>();
    public bool openSelectionChild=false;
}
