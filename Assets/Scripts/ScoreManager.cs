using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance giúp truy cập dễ dàng từ các script khác
    public static ScoreManager Instance { get; private set; }

    // Điểm số hiện tại của lượt chơi
    public int CurrentScore { get; private set; } = 0;

    // Điểm kỷ lục cao nhất của độ khó hiện tại
    public int HighScore { get; private set; } = 0;

    private int currentDifficulty = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        // Đăng ký nhận sự kiện cộng điểm từ ScoringZone
        ScoringZone.OnScored += HandleScored;
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện khi object bị vô hiệu hóa/xóa để tránh memory leak
        ScoringZone.OnScored -= HandleScored;
    }

    private void HandleScored()
    {
        AddScore(1);
    }

    private void Start()
    {
        // Lấy mức độ khó hiện tại từ PlayerPrefs (0: Dễ, 1: Trung bình, 2: Khó)
        currentDifficulty = PlayerPrefs.GetInt("DifficultyLevel", 0);

        // Tải HighScore tương ứng với độ khó đó
        HighScore = PlayerPrefs.GetInt($"HighScore_{currentDifficulty}", 0);

        ResetScore();
    }

    /// <summary>
    /// Cộng điểm số khi chim bay qua ống thành công
    /// </summary>
    public void AddScore(int amount = 1)
    {
        CurrentScore += amount;

        // Cập nhật giao diện HUD điểm số đang chơi
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(CurrentScore);
        }

        // Phát âm thanh khi tính điểm
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScoreSFX();
        }
    }

    /// <summary>
    /// Đưa điểm số lượt chơi hiện tại về 0 khi bắt đầu ván mới
    /// </summary>
    public void ResetScore()
    {
        CurrentScore = 0;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(CurrentScore);
        }
    }

    /// <summary>
    /// Kiểm tra và lưu điểm kỷ lục mới nếu điểm hiện tại cao hơn kỷ lục cũ
    /// </summary>
    public void CheckAndSaveHighScore()
    {
        currentDifficulty = PlayerPrefs.GetInt("DifficultyLevel", 0);
        string key = $"HighScore_{currentDifficulty}";

        // Lấy điểm kỷ lục cũ đã lưu
        int savedHighScore = PlayerPrefs.GetInt(key, 0);

        // Nếu điểm hiện tại lớn hơn kỷ lục cũ, tiến hành cập nhật và lưu
        if (CurrentScore > savedHighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(key, HighScore);
            PlayerPrefs.Save();
        }
        else
        {
            HighScore = savedHighScore;
        }
    }
}