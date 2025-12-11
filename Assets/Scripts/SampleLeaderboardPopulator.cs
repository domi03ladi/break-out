using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SampleLeaderboardPopulator : MonoBehaviour
{
    [SerializeField] private LeaderboardManager leaderboardManager;

    void Start()
    {
        // Sample player data
        List<LeaderboardEntry> sampleData = new List<LeaderboardEntry>()
        {
            new LeaderboardEntry { playerName = "Alex", score = 950, lvl = "Level 3" },
            new LeaderboardEntry { playerName = "Jordan", score = 880, lvl = "Level 2" },
            new LeaderboardEntry { playerName = "Casey", score = 870, lvl = "Level 4" },
            new LeaderboardEntry { playerName = "Taylor", score = 840, lvl = "Level 1" },
            new LeaderboardEntry { playerName = "Riley", score = 800, lvl = "Level 3" },
            new LeaderboardEntry { playerName = "Morgan", score = 780, lvl = "Level 2" },
            new LeaderboardEntry { playerName = "Jamie", score = 760, lvl = "Level 1" },
            new LeaderboardEntry { playerName = "Sam", score = 740, lvl = "Level 4" },
            new LeaderboardEntry { playerName = "Chris", score = 720, lvl = "Level 2" },
            new LeaderboardEntry { playerName = "Pat", score = 700, lvl = "Level 1" }
        };

        leaderboardManager.ClearLeaderboard(); // <- Clear previous data first

        foreach (LeaderboardEntry entry in sampleData)
        {
            leaderboardManager.AddScore(entry.playerName, entry.score, entry.lvl);
        }

        leaderboardManager.PopulateUI();
    }
}
