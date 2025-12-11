using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject entryPrefab;

    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    void OnEnable()
    {
        LoadLeaderboard();
        PopulateUI();
    }

    public void AddScore(string playerName, int score, string lvl)
    {
        entries.Add(new LeaderboardEntry
        {
            playerName = playerName,
            score = score,
            lvl = lvl
        });

        SaveLeaderboard();
    }

    public void PopulateUI()
    {
        // Clear old entries
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Sort highest score first
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Create UI rows
        for (int i = 0; i < entries.Count; i++)
        {
            var obj = Instantiate(entryPrefab, contentParent);
            TextMeshProUGUI[] fields = obj.GetComponentsInChildren<TextMeshProUGUI>();

            fields[0].text = (i + 1).ToString();               
            fields[1].text = entries[i].playerName;            
            fields[2].text = entries[i].score.ToString();
            fields[3].text = entries[i].lvl.ToString();
        }
    }

    private void SaveLeaderboard()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            PlayerPrefs.SetString("LB_Name_" + i, entries[i].playerName);
            PlayerPrefs.SetInt("LB_Score_" + i, entries[i].score);
            PlayerPrefs.SetString("LB_Lvl_" + i, entries[i].lvl);   
        }

        PlayerPrefs.SetInt("LB_Count", entries.Count);
    }

    private void LoadLeaderboard()
    {
        entries.Clear();
        int count = PlayerPrefs.GetInt("LB_Count", 0);

        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString("LB_Name_" + i, "---");
            int score = PlayerPrefs.GetInt("LB_Score_" + i, 0);
            string lvl = PlayerPrefs.GetString("LB_Lvl_" + i, "---");   

            entries.Add(new LeaderboardEntry
            {
                playerName = name,
                score = score,
                lvl = lvl
            });
        }
    }

    public void ClearLeaderboard()
    {
        int count = PlayerPrefs.GetInt("LB_Count", 0);
        for (int i = 0; i < count; i++)
        {
            PlayerPrefs.DeleteKey("LB_Name_" + i);
            PlayerPrefs.DeleteKey("LB_Score_" + i);
            PlayerPrefs.DeleteKey("LB_Lvl_" + i);
        }
        PlayerPrefs.DeleteKey("LB_Count");

        entries.Clear();
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public string lvl;   
}
