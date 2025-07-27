using TMPro;
using UnityEngine;

public class WinSceneScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        int finalScore = ScoreManager.Instance.GetScore();
        scoreText.text = "Your Score: " + finalScore;
        TopScoreManager.Instance.AddScore(finalScore);
    }
}
