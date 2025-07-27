using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TopScoreManager : MonoBehaviour
{
    public static TopScoreManager Instance;

    private const string SaveKey = "TopScores";
    public int maxScores = 5;

    public List<ScoreEntry> topScores = new List<ScoreEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int score)
    {
        ScoreEntry entry = new ScoreEntry(score);
        topScores.Add(entry);

        // Sắp xếp giảm dần theo điểm, lấy top N
        topScores = topScores
            .OrderByDescending(e => e.score)
            .Take(maxScores)
            .ToList();

        SaveScores();
    }

    public void SaveScores()
    {
        string json = JsonUtility.ToJson(new ScoreListWrapper(topScores));
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void LoadScores()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            ScoreListWrapper wrapper = JsonUtility.FromJson<ScoreListWrapper>(json);
            topScores = wrapper.entries;
        }
    }

    [System.Serializable]
    private class ScoreListWrapper
    {
        public List<ScoreEntry> entries;

        public ScoreListWrapper(List<ScoreEntry> entries)
        {
            this.entries = entries;
        }
    }
}
