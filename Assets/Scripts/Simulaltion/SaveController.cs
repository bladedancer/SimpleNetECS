using Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public SimulationController simCtrl;

    // Make shared component serializable seems to do bad things so have 
    // save type here.
    [Serializable]
    public class SaveData
    {
        public int Tag;
        public float Fitness;
        public int[] LayerSizes;
        public double[] Weights;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveFittest();
        }        
    }

    public void SaveFittest()
    {
        if (simCtrl.CurrentFittestNet != null)
        {
            Debug.Log("Saving");
            // TODO NAMING ETC
            Nullable<Fittest> fittest = simCtrl.Fittest;
            Stats currentFittestStats = simCtrl.CurrentFittest.Value;
            Nullable<NetData> data = null;
            int tag;
            float fitness;

            if (fittest.HasValue && currentFittestStats.Fitness < fittest.Value.fitness)
            {
                data = fittest.Value.net;
                tag = fittest.Value.tag;
                fitness = fittest.Value.fitness;
            }
            else
            {
                data = simCtrl.CurrentFittestNet.Data;
                tag = currentFittestStats.Tag;
                fitness = currentFittestStats.Fitness;
            }

            string loc = Save("cow", fitness, tag, data.Value);
            if (loc != null)
            {
                Debug.Log("Saved: " + loc);
            }
        }
    }

    private string Save(string name, float fitness, int tag, NetData data)
    {
        SaveData save = new SaveData
        {
            Fitness = fitness,
            Tag = tag,
            LayerSizes = data.LayerSizes,
            Weights = data.Weights
        };

        string path = GetSavePath(name);
        File.WriteAllText(path, JsonUtility.ToJson(save, true));
        return path;
    }

    public SaveData Load(string name)
    {
        if (!DoesSaveExist(name))
        {
            return null;
        }

        SaveData loadedData = null;
        string path = GetSavePath(name);

        if (File.Exists(path))
        {
            Debug.Log("Loading: " + path);
            string dataAsJson = File.ReadAllText(path);
            loadedData = JsonUtility.FromJson<SaveData>(dataAsJson);
            return loadedData;
        }
        else
        {
            Debug.Log("No saves.");
        }

        return loadedData;
    }

    public bool DeleteSaveGame(string name)
    {
        try
        {
            Debug.Log("Deleting: " + GetSavePath(name));
            File.Delete(GetSavePath(name));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public bool DoesSaveExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    private string GetSavePath(string name)
    {
        return Path.Combine(Application.persistentDataPath, name + ".json");
    }
}
