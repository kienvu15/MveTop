[System.Serializable]
public class ScoreEntry
{
    public string dateTime;  // ví dụ: "2025-07-27 11:30"
    public int score;

    public ScoreEntry(int score)
    {
        this.dateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        this.score = score;
    }
}
