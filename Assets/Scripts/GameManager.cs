using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance quản lý luồng game
    public static GameManager Instance { get; private set; }

    // Cờ trạng thái kết thúc game
    public bool IsGameOver { get; private set; } = false;

    [Header("Tham Chiếu Hệ Thống")]
    [SerializeField] private PlayerController player;
    [SerializeField] private PipeSpawner pipeSpawner;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Hình Nền Theo Cấp Độ")]
    [SerializeField] private Sprite bgEasy;
    [SerializeField] private Sprite bgMedium;
    [SerializeField] private Sprite bgHard;

    [Header("Thông Số Tốc Độ Game")]
    public float gameSpeed = 3.0f;

    private Vector3 playerStartPosition;

    private void Awake()
    {
        // Khởi tạo Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Phát nhạc nền Gameplay khi mở Scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGamePlayBGM();
        }

        // Lưu lại vị trí ban đầu của Player để Reset khi chơi lại
        if (player != null)
        {
            playerStartPosition = player.transform.position;
        }

        StartGame();
    }

    /// <summary>
    /// Bắt đầu hoặc tái khởi động ván chơi mới
    /// </summary>
    public void StartGame()
    {
        Time.timeScale = 1f;
        IsGameOver = false;

        // Reset xu và điểm số ván chơi hiện tại
        if (CoinManager.Instance != null) CoinManager.Instance.ResetSessionCoins();
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();

        // Ép cập nhật lại giao diện HUD khi bắt đầu ván mới
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(0);
            UIManager.Instance.UpdateCoinUI(0);
        }

        int difficulty = PlayerPrefs.GetInt("DifficultyLevel", 0);
        ApplyDifficultySettings(difficulty);

        if (pipeSpawner != null)
        {
            pipeSpawner.SetDifficulty(difficulty);
            pipeSpawner.ResetSpawner();
        }

        if (player != null) player.ResetPlayer(playerStartPosition);

        if (UIManager.Instance != null) UIManager.Instance.ShowGameOverPanel(false);
    }

    /// <summary>
    /// Áp dụng cấu hình tốc độ và Sprite hình nền tương ứng độ khó
    /// </summary>
    private void ApplyDifficultySettings(int level)
    {
        switch (level)
        {
            case 0: // Dễ
                gameSpeed = 2.5f;
                if (backgroundRenderer != null && bgEasy != null) backgroundRenderer.sprite = bgEasy;
                break;
            case 1: // Trung bình
                gameSpeed = 3.5f;
                if (backgroundRenderer != null && bgMedium != null) backgroundRenderer.sprite = bgMedium;
                break;
            case 2: // Khó
                gameSpeed = 4.5f;
                if (backgroundRenderer != null && bgHard != null) backgroundRenderer.sprite = bgHard;
                break;
        }
    }

    /// <summary>
    /// Xử lý sự kiện khi nhân vật va chạm và thua cuộc
    /// </summary>
    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f; // Dừng thời gian game

        // Lưu số xu tích lũy thu thập được
        if (CoinManager.Instance != null) CoinManager.Instance.SaveTotalCoins();

        // Hiển thị bảng GameOver (UIManager sẽ tự gọi CheckAndSaveHighScore)
        if (UIManager.Instance != null) UIManager.Instance.ShowGameOverPanel(true);

        // Phát hiệu ứng âm thanh va chạm
        if (AudioManager.Instance != null) AudioManager.Instance.PlayHitSFX();
    }

    /// <summary>
    /// Nút Chơi Lại (RETRY) trên GameOver Panel
    /// </summary>
    public void RestartGame()
    {
        StartGame();
    }

    /// <summary>
    /// Nút Trở Về Menu Chính
    /// </summary>
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}