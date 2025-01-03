using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct RelicsInfo
{
    public int idx;
    public string name;
    public int floorNumber;
    public int relicsNumber;
    public string age;
    public string material;
    public string destination;
    public string summary;
    public string description;
}

[Serializable]
public struct RelicsData
{
    public List<RelicsInfo> relicsData;
}

public class RelicsDataLoader : MonoBehaviour
{
    void Awake()
    {
        SetRelicsData();
    }
    
    void SetRelicsData()
    {
        string relicsDataJsonString = File.ReadAllText(Application.streamingAssetsPath + "/relicsData.json");
        relicsDataJsonString = "{\n" + "\"" + "relicsData" + "\" :\n" + relicsDataJsonString + "}";
        GameInstance.Instance.RelicsDataManager.relicsData = JsonUtility.FromJson<RelicsData>(relicsDataJsonString);
    }
}
