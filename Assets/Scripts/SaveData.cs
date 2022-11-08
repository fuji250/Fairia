using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData : MonoBehaviour
{
    [System.Serializable]
    public class FailedData
    {
        public int failedNum;
        
        public List<int> currentRight = new List<int>();
        public List<string> rightLists = new List<string>();
        public List<int> currentLeft = new List<int>();
        public List<string> leftLists = new List<string>();
        public List<int> currentJump = new List<int>();
        public List<string> jumpLists = new List<string>();
    }

    private void Awake()
    {
        SaveData.FailedData failedData = new SaveData.FailedData();
        //SaveData.SavePlayerData(failedData);
    }

    public static void SavePlayerData(FailedData failedData)
    {
        StreamWriter writer;
 
        string jsonstr = JsonUtility.ToJson (failedData);
 
        writer = new StreamWriter(Application.dataPath + "/savedata.json", false);
        writer.Write (jsonstr);
        writer.Flush ();
        writer.Close ();
    }
    
    public static FailedData LoadPlayerData()
    {
        if (!File.Exists(Application.dataPath + "/savedata.json"))
        {
            SavePlayerData(new SaveData.FailedData());
        }
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader (Application.dataPath + "/savedata.json");
        datastr = reader.ReadToEnd ();
        reader.Close ();
 
        return JsonUtility.FromJson<FailedData> (datastr);
    }
}
