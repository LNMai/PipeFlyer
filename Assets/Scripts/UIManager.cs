using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quản lý toàn bộ giao diện người dùng (UI) trong màn chơi bao gồm HUD và bảng Game Over.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI GameOver Text References")]
    [SerializeField] private TMP_Text finalScoreText;     // Text hiển thị điểm đạt được khi Game Over
    [SerializeField] private TMP_Text highScoreText;      // Text hiển thị kỷ lục điểm cao nhất

    [Header("UI Panel & Button References")]
    [SerializeField] private GameObject gameOverPanel;    // Panel bảng Game Over
    [SerializeField] private Button restartButton;        // Nút chơi lại (Retry)
    [SerializeField] private Button backButton;           // Nút quay về trang chủ (Home)

    [Header("In-Game HUD References")]
    [SerializeField] private TMP_Text coinText;           // Text hiển thị số xu thu thập trong trận
    [SerializeField] private TMP_Text scoreText;          // Text hiển thị điểm hiện tại trong trận

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

    private void OnEnable()
    {
        // Đăng ký nhận sự kiện cập nhật xu từ CoinManager
        CoinManager.OnSessionCoinsChanged += UpdateCoinUI;
    }

    private void OnDisable()
    {
        // Hủy đăng ký khi object bị ẩn/hủy để tránh memory leak
        CoinManager.OnSessionCoinsChanged -= UpdateCoinUI;
    }

    private void Start()
    {
        // Gắn sự kiện click cho nút Chơi lại
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        // Gắn sự kiện click cho nút Trở về Menu chính
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        // Đảm bảo đồng bộ lại HUD ngay khi Scene khởi chạy
        if (ScoreManager.Instance != null)
            UpdateScoreUI(ScoreManager.Instance.CurrentScore);

        if (CoinManager.Instance != null)
            UpdateCoinUI(CoinManager.Instance.SessionCoins);
    }

    // Cập nhật giá trị điểm lên UI Gameplay
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    // Cập nhật giá trị xu lên UI Gameplay
    public void UpdateCoinUI(int coins)
    {
        if (coinText != null) coinText.text = coins.ToString();
    }

    // Hiển thị hoặc ẩn Panel Game Over và cập nhật kết quả điểm
    public void ShowGameOverPanel(bool show)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(show);
        }
        if (show)
        {
            // Kiểm tra điểm số và lưu kỷ lục mới nếu có khi hiển thị bảng Game Over
            if (ScoreManager.Instance != null)
            {
                if (finalScoreText != null)
                    finalScoreText.text = "Score: " + ScoreManager.Instance.CurrentScore;

                ScoreManager.Instance.CheckAndSaveHighScore();

                if (highScoreText != null)
                    highScoreText.text = "Best: " + ScoreManager.Instance.HighScore;
            }
        }
    }

    // Xử lý sự kiện khi ấn nút Chơi lại (Restart/Retry)
    private void OnRestartButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSFX();
        if (GameManager.Instance != null) GameManager.Instance.RestartGame();
    }

    // Xử lý sự kiện khi ấn nút Trở về Main Menu (Home)
    private void OnBackButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSFX();
        if (GameManager.Instance != null) GameManager.Instance.BackToMainMenu();
    }
}