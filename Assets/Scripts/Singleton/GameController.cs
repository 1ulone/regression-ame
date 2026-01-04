using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; 
using System.IO;

public class GameController : MonoBehaviour
{
    public static GameController instances;
    private string path;

    private void Awake()
    {
        instances = this;
        path = Application.persistentDataPath + "/save.sav";
    }

    public bool CheckForFile()
    { return File.Exists(path); }

    public SaveData LoadData()
    {
        if (!File.Exists(path))
        {
            SaveData ndata = new SaveData(5);
            WriteSave(ndata);
            return ndata;
        }

        string saveFile = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(saveFile);

        return data;
    }

    public void WriteSave(SaveData data)
    {
        string textToSave = JsonUtility.ToJson(data);
        File.WriteAllText(path, textToSave);
    }


    public void ClearSave()
    {
        if (!File.Exists(path))
            return;

        File.Delete(path);
    }

    public void RemoveSkill(PlayerBuffData data)
    {
        SaveData sav = LoadData();
        sav.playerSkill.Remove(data);
        WriteSave(sav);
    }

    public void AddSkill(PlayerBuffData data)
    {
        SaveData sav = LoadData();
        sav.playerSkill.Add(data);
        WriteSave(sav);
    }

    public void AddTime(int time)
    {
        SaveData sav = LoadData();
        sav.time += time;
        WriteSave(sav);
    }
    
    public void RestartLevel()
    {
        StartCoroutine(restartTransition());
    }

    private IEnumerator restartTransition()
    {
        SaveData data = LoadData();
        yield return SceneManager.UnloadSceneAsync(2);

        yield return SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        yield return new WaitForSecondsRealtime(0.05f);

        GameObject.FindFirstObjectByType<TimeController>().countdown = data.time;
        PlayerController p = GameObject.FindFirstObjectByType<PlayerController>();
        p.buffs = data.playerSkill;
        p.UpdatePlayerStats();

        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1;
    }
}

[System.Serializable]
public class SaveData
{
    public int time;
    public List<PlayerBuffData> playerSkill;

    public SaveData(int t)
    {
        time = t;
        playerSkill = new List<PlayerBuffData>();
    }
}
