using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quản lý tương tác và giao diện màn hình Menu Chính (Main Menu).
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button btnEasy;     // Nút chọn mức Dễ
    [SerializeField] private Button btnMedium;   // Nút chọn mức Trung Bình
    [SerializeField] private Button btnHard;     // Nút chọn mức Khó
    [SerializeField] private Button btnShop;     // Nút mở Cửa hàng (Shop)

    [Header("Best Score Texts Theo Độ Khó")]
    [SerializeField] private TMP_Text bestEasyText;   // Text điểm cao nhất - Dễ
    [SerializeField] private TMP_Text bestMediumText; // Text điểm cao nhất - Trung Bình
    [SerializeField] private TMP_Text bestHardText;   // Text điểm cao nhất - Khó

    [Header("Total Coins Text")]
    [SerializeField] private TMP_Text totalCoinText;  // Text hiển thị Tổng Xu tích lũy

    private void Start()
    {
        // Phát nhạc nền Menu
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }

        UpdateMainMenuUI();

        // Đăng ký sự kiện Click cho các nút độ khó
        if (btnEasy != null) btnEasy.onClick.AddListener(() => StartGameWithDifficulty(0));
        if (btnMedium != null) btnMedium.onClick.AddListener(() => StartGameWithDifficulty(1));
        if (btnHard != null) btnHard.onClick.AddListener(() => StartGameWithDifficulty(2));

        // Đăng ký sự kiện mở Shop
        if (btnShop != null)
        {
            btnShop.onClick.AddListener(() => {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSFX();
                if (ShopManager.Instance != null) ShopManager.Instance.OpenShop();
            });
        }
    }

    /// <summary>
    /// Cập nhật thông số Tổng xu và Điểm kỷ lục ở từng mức độ khó lên UI
    /// </summary>
    public void UpdateMainMenuUI()
    {
        // 1. Cập nhật Tổng Xu
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        if (totalCoinText != null) totalCoinText.text = totalCoins.ToString();

        // 2. Cập nhật Kỷ lục cho từng độ khó (0: Dễ, 1: Trung bình, 2: Khó)
        int bestEasy = PlayerPrefs.GetInt("HighScore_0", 0);
        int bestMedium = PlayerPrefs.GetInt("HighScore_1", 0);
        int bestHard = PlayerPrefs.GetInt("HighScore_2", 0);

        if (bestEasyText != null) bestEasyText.text = "Best: " + bestEasy;
        if (bestMediumText != null) bestMediumText.text = "Best: " + bestMedium;
        if (bestHardText != null) bestHardText.text = "Best: " + bestHard;
    }

    /// <summary>
    /// Lưu mức độ khó đã chọn và load Scene GamePlay
    /// </summary>
    private void StartGameWithDifficulty(int level)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSFX();
        PlayerPrefs.SetInt("DifficultyLevel", level);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GamePlayScene");
    }
}