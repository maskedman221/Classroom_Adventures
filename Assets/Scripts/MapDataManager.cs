using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Collections; 
public class MapDataManager : MonoBehaviour
{
    public static MapDataManager Instance { get; private set; }
    public MapData Data = new MapData();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //string token = SecurePlayerPrefs.GetEncryptedString("auth_token");
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadGameData();
       
        // if (!string.IsNullOrEmpty(token))
        // {
        //     if (Data.childId != null)
        //     {
        //         SceneManager.LoadScene(3);
        //     }
        //     else
        //     {
        //         openSelectionChild = true;
        //     }
        // }

    }
    private void Start()
    {
        string token = SecurePlayerPrefs.GetEncryptedString("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            if (Data.childId > 0)
                SceneManager.LoadScene(3);
            else
                Data.openSelectionChild = true;
        }
    }
    public void SetChildId(int id)
    {
        Data.childId = id;
        SaveGameData();
    }
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    // private static void AutoInit()
    // {
    //     if (Instance == null)
    //     {
    //         GameObject prefab = Resources.Load<GameObject>("MapDataManager");
    //         if (prefab != null)
    //         {
    //             GameObject obj = GameObject.Instantiate(prefab);
    //             obj.name = "MapDataObject";
    //         }
    //         else
    //         {
    //             Debug.LogError("❌ MapDataManager prefab not found in Resources!");
    //         }
    //     }
    // }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("App paused → save progress");
            SaveGameData();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("App quit → save progress");
        SaveGameData();
    }

    private void SaveGameData()
    {

        string json = JsonUtility.ToJson(Data);
        SecurePlayerPrefs.SaveEncryptedString("mapData", json);
    }
    private void LoadGameData()
    {
        string json = SecurePlayerPrefs.GetEncryptedString("mapData");
        if (!string.IsNullOrEmpty(json))
        {
            Data = JsonUtility.FromJson<MapData>(json);
            Debug.Log("Game data loaded");
        }
        else
        {
            Debug.Log("No saved data found, starting fresh");
            Data = new MapData();
        }
    }
}
