using UnityEngine;
using TMPro;

public class TopScoreUI : MonoBehaviour
{
    public TMP_Text topScoreText;

    void Start()
    {
        var entries = TopScoreManager.Instance.topScores;

        string text = "Top Scores:\n";
        foreach (var entry in entries)
        {
            text += $"{entry.dateTime} - {entry.score} pts\n";
        }

        topScoreText.text = text;
    }
}
