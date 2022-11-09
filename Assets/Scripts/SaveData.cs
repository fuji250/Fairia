using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class SaveData : MonoBehaviour
{
    [System.Serializable]
    public class FailedData
    {
        public int failedNum;
        public List<string> rightLists = new List<string>();
        public List<string> jumpLists = new List<string>();

        public int clearSta;
    }

    
    
    public static void SavePlayerData(FailedData failedData)
    {
        StreamWriter writer;
 
        string jsonstr = JsonUtility.ToJson (failedData);
 
        writer = new StreamWriter(Application.persistentDataPath + "/savedata.json", false);
        writer.Write (jsonstr);
        writer.Flush ();
        writer.Close ();
    }
    
    public static FailedData LoadPlayerData()
    {
        if (!File.Exists(Application.persistentDataPath + "/savedata.json"))
        {
            SavePlayerData(new SaveData.FailedData());
        }
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader (Application.persistentDataPath + "/savedata.json");
        datastr = reader.ReadToEnd ();
        reader.Close ();
 
        return JsonUtility.FromJson<FailedData> (datastr);
    }
}
