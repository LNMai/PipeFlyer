using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quản lý giao diện, giao dịch mua bán và chọn skin trong Shop.
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Danh Sách Skin")]
    public List<SkinItemData> skinList = new List<SkinItemData>();

    [Header("UI References")]
    [SerializeField] private TMP_Text shopTotalCoinText;
    [SerializeField] private Image skinPreviewImage;
    [SerializeField] private TMP_Text skinNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image coinIcon;               // Tham chiếu Icon Xu bên cạnh giá tiền
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionButtonText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private GameObject shopPanel;

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private CanvasGroup notificationCanvasGroup;

    private int currentDisplayIndex = 0;                    // Chỉ số skin đang hiển thị
    private const string SELECTED_SKIN_KEY = "SelectedSkinIndex";
    private Coroutine notificationCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Skin đầu tiên luôn được mở khóa mặc định
        PlayerPrefs.SetInt("SkinUnlocked_0", 1);
        currentDisplayIndex = PlayerPrefs.GetInt(SELECTED_SKIN_KEY, 0);

        // Đăng ký sự kiện Click cho các nút UI
        if (nextButton != null) nextButton.onClick.AddListener(NextSkin);
        if (prevButton != null) prevButton.onClick.AddListener(PrevSkin);
        if (actionButton != null) actionButton.onClick.AddListener(OnActionButtonClicked);

        UpdateShopUI();
    }

    // Hiển thị Panel Shop và cập nhật lại giao diện
    public void OpenShop()
    {
        if (shopPanel != null) shopPanel.SetActive(true);
        currentDisplayIndex = PlayerPrefs.GetInt(SELECTED_SKIN_KEY, 0);
        UpdateShopUI();
    }

    // Ẩn Panel Shop
    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    // Chuyển sang Skin tiếp theo trong danh sách
    private void NextSkin()
    {
        if (skinList == null || skinList.Count == 0) return;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSFX();
        currentDisplayIndex = (currentDisplayIndex + 1) % skinList.Count;
        UpdateShopUI();
    }

    // Quay lại Skin phía trước trong danh sách
    private void PrevSkin()
    {
        if (skinList == null || skinList.Count == 0) return;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSFX();
        currentDisplayIndex--;
        if (currentDisplayIndex < 0) currentDisplayIndex = skinList.Count - 1;
        UpdateShopUI();
    }

    // Cập nhật toàn bộ thông tin UI của Skin đang chọn
    public void UpdateShopUI()
    {
        if (skinList == null || skinList.Count == 0)
        {
            Debug.LogWarning("[ShopManager] Cần thêm ít nhất 1 Skin vào danh sách Skin List trên Inspector!");
            return;
        }

        if (currentDisplayIndex >= skinList.Count) currentDisplayIndex = 0;

        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        if (shopTotalCoinText != null) shopTotalCoinText.text = totalCoins.ToString();

        SkinItemData currentSkin = skinList[currentDisplayIndex];

        if (skinPreviewImage != null) skinPreviewImage.sprite = currentSkin.skinSprite;
        if (skinNameText != null) skinNameText.text = currentSkin.skinName;

        bool isUnlocked = PlayerPrefs.GetInt("SkinUnlocked_" + currentDisplayIndex, 0) == 1;
        int selectedIndex = PlayerPrefs.GetInt(SELECTED_SKIN_KEY, 0);

        if (isUnlocked)
        {
            // Ẩn cả text giá tiền và icon xu khi skin đã mua/được mở khóa
            if (priceText != null) priceText.gameObject.SetActive(false);
            if (coinIcon != null) coinIcon.gameObject.SetActive(false);

            if (selectedIndex == currentDisplayIndex)
            {
                // Đang sử dụng skin này
                if (actionButtonText != null) actionButtonText.text = "Selected";
                if (actionButton != null) actionButton.interactable = false;
            }
            else
            {
                // Đã mở khóa nhưng chưa dùng
                if (actionButtonText != null) actionButtonText.text = "Select";
                if (actionButton != null) actionButton.interactable = true;
            }
        }
        else
        {
            // Chưa mua: Hiển thị lại text giá tiền và icon xu
            if (priceText != null)
            {
                priceText.gameObject.SetActive(true);
                priceText.text = currentSkin.price.ToString();
            }

            if (coinIcon != null)
            {
                coinIcon.gameObject.SetActive(true);
            }

            if (actionButtonText != null) actionButtonText.text = "Buy";
            if (actionButton != null) actionButton.interactable = (totalCoins >= currentSkin.price);
        }
    }

    // Xử lý sự kiện khi ấn nút hành động (Mua / Chọn Skin)
    private void OnActionButtonClicked()
    {
        if (skinList == null || skinList.Count == 0) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSFX();

        bool isUnlocked = PlayerPrefs.GetInt("SkinUnlocked_" + currentDisplayIndex, 0) == 1;
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        SkinItemData currentSkin = skinList[currentDisplayIndex];

        if (!isUnlocked)
        {
            // Xử lý logic mua Skin
            if (totalCoins >= currentSkin.price)
            {
                totalCoins -= currentSkin.price;
                PlayerPrefs.SetInt("TotalCoins", totalCoins);
                PlayerPrefs.SetInt("SkinUnlocked_" + currentDisplayIndex, 1);
                PlayerPrefs.SetInt(SELECTED_SKIN_KEY, currentDisplayIndex);
                PlayerPrefs.Save();

                ShowNotification("Skin unlocked!");
            }
            else
            {
                ShowNotification("Not enough coins!");
            }
        }
        else
        {
            // Xử lý logic chọn Skin để trang bị
            PlayerPrefs.SetInt(SELECTED_SKIN_KEY, currentDisplayIndex);
            PlayerPrefs.Save();
        }

        UpdateShopUI();
    }

    // Lấy Sprite của Skin hiện đang được chọn để trang bị cho Player
    public Sprite GetSelectedSkinSprite()
    {
        if (skinList == null || skinList.Count == 0) return null;

        int selectedIndex = PlayerPrefs.GetInt(SELECTED_SKIN_KEY, 0);
        if (selectedIndex >= 0 && selectedIndex < skinList.Count)
        {
            return skinList[selectedIndex].skinSprite;
        }
        return skinList[0].skinSprite;
    }

    // Kích hoạt Coroutine hiển thị thông báo
    public void ShowNotification(string message, float duration = 1.5f)
    {
        if (notificationPanel == null || notificationText == null) return;

        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = StartCoroutine(Co_ShowNotification(message, duration));
    }

    // Coroutine xử lý hiệu ứng mờ dần (Fade Out) cho Panel thông báo
    private IEnumerator Co_ShowNotification(string message, float duration)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);

        if (notificationCanvasGroup != null)
        {
            notificationCanvasGroup.alpha = 1f;
        }

        yield return new WaitForSecondsRealtime(duration);

        if (notificationCanvasGroup != null)
        {
            float fadeTime = 0.3f;
            float elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                notificationCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                yield return null;
            }
        }

        notificationPanel.SetActive(false);
    }
}