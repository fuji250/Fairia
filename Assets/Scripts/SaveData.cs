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
        
        public List<List<bool>> rightLists = new List<List<bool>>();
        public List<bool> currentRight = new List<bool>();
        public string rightString;
        public int rightInt;
        public List<int> rightInts = new List<int>();
    }

    private void Awake()
    {
        SaveData.FailedData failedData = new SaveData.FailedData();
        SaveData.SavePlayerData(failedData);
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
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader (Application.dataPath + "/savedata.json");
        datastr = reader.ReadToEnd ();
        reader.Close ();
 
        return JsonUtility.FromJson<FailedData> (datastr);
    }
}
