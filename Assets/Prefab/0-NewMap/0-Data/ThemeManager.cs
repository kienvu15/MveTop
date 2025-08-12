using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public ThemeType currentTheme;
    public ThemeType lastTheme = ThemeType.None;
    public int stageIndexInTheme = 1;

    private void Start()
    {

        currentTheme = GetRandomThemeExcluding(lastTheme);
        lastTheme = currentTheme;
    }

    /// <summary>
    /// Tăng stage trong theme hiện tại.
    /// Nếu đã tới stage 3 thì tự động chuyển theme mới.
    /// </summary>
    public void NextStage()
    {
        stageIndexInTheme++;

        if (stageIndexInTheme > 3)
        {
            ChangeThemeAfterBoss();
        }
    }

    /// <summary>
    /// Chuyển sang theme mới và reset stage về 1.
    /// Dùng khi qua boss hoặc muốn đổi theme ngay lập tức.
    /// </summary>
    public void ChangeThemeAfterBoss()
    {
        stageIndexInTheme = 1;
        currentTheme = GetRandomThemeExcluding(currentTheme);
        lastTheme = currentTheme;
        Debug.Log($"[ThemeManager] Chuyển sang theme mới: {currentTheme}");
    }

    /// <summary>
    /// Lấy theme ngẫu nhiên, loại trừ một theme nhất định.
    /// </summary>
    public ThemeType GetRandomThemeExcluding(ThemeType exclude)
    {
        ThemeType[] allThemes = (ThemeType[])System.Enum.GetValues(typeof(ThemeType));
        ThemeType[] possibleThemes = System.Array.FindAll(allThemes, t => t != exclude && t != ThemeType.None);
        return possibleThemes[Random.Range(0, possibleThemes.Length)];
    }
}
