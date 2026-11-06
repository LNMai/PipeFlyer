using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    // Events để UIManager/AudioManager đăng ký lắng nghe (Loại bỏ tight coupling)
    public static event Action<int> OnSessionCoinsChanged;
    public static event Action OnCoinCollected;

    public int SessionCoins { get; private set; } = 0;
    public int TotalCoins { get; private set; } = 0;

    private const string TOTAL_COINS_KEY = "TotalCoins";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadTotalCoins();
    }

    private void Start()
    {
        ResetSessionCoins();
    }

    /// <summary>
    /// Thêm coin vào session hiện tại (vd: nhặt trong khi chơi)
    /// </summary>
    public void AddCoins(int amount = 1)
    {
        if (amount <= 0) return;

        SessionCoins += amount;

        OnCoinCollected?.Invoke();
        OnSessionCoinsChanged?.Invoke(SessionCoins);
    }

    /// <summary>
    /// Cộng dồn session coin vào total coin và lưu vào PlayerPrefs
    /// </summary>
    public void SaveTotalCoins()
    {
        TotalCoins += SessionCoins;
        PlayerPrefs.SetInt(TOTAL_COINS_KEY, TotalCoins);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Trừ TotalCoins khi mua vật phẩm trong Shop
    /// </summary>
    public bool TrySpendCoins(int price)
    {
        if (price <= 0 || TotalCoins < price) return false;

        TotalCoins -= price;
        PlayerPrefs.SetInt(TOTAL_COINS_KEY, TotalCoins);
        PlayerPrefs.Save();
        return true;
    }

    /// <summary>
    /// Reset lại số coin của màn chơi hiện tại
    /// </summary>
    public void ResetSessionCoins()
    {
        SessionCoins = 0;
        OnSessionCoinsChanged?.Invoke(SessionCoins);
    }

    private void LoadTotalCoins()
    {
        TotalCoins = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
    }
}