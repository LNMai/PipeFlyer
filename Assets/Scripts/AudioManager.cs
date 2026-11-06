using System;
using UnityEngine;

/// <summary>
/// Quản lý âm thanh toàn cục (BGM & SFX) dạng Singleton, không bị xóa khi chuyển Scene.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // Nguồn phát nhạc nền
    [SerializeField] private AudioSource sfxSource; // Nguồn phát hiệu ứng âm thanh

    [Header("Background Music")]
    [SerializeField] private AudioClip mainMenuBGM; // Nhạc nền Main Menu
    [SerializeField] private AudioClip gamePlayBGM; // Nhạc nền Game Play

    [Header("Sound Effects")]
    [SerializeField] private AudioClip clickSFX; // Âm thanh click nút UI
    [SerializeField] private AudioClip coinSFX;  // Âm thanh ăn xu
    [SerializeField] private AudioClip scoreSFX; // Âm thanh qua cột cộng điểm
    [SerializeField] private AudioClip hitSFX;   // Âm thanh va chạm (thua)
    [SerializeField] private AudioClip flapSFX;  // Âm thanh vỗ cánh (nhảy)

    private void Awake()
    {
        // Khởi tạo Singleton và giữ lại object khi load scene mới
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Phát nhạc nền cho màn hình Menu Chính
    /// </summary>
    public void PlayMainMenuBGM()
    {
        if (bgmSource != null && mainMenuBGM != null)
        {
            if (bgmSource.clip == mainMenuBGM && bgmSource.isPlaying) return;
            bgmSource.clip = mainMenuBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// Phát nhạc nền cho màn chơi GamePlay
    /// </summary>
    public void PlayGamePlayBGM()
    {
        if (bgmSource != null && gamePlayBGM != null)
        {
            if (bgmSource.clip == gamePlayBGM && bgmSource.isPlaying) return;
            bgmSource.clip = gamePlayBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // Các hàm phát hiệu ứng âm thanh một lần (PlayOneShot)

    public void PlayClickSFX()
    {
        if (sfxSource != null && clickSFX != null)
            sfxSource.PlayOneShot(clickSFX);
    }

    public void PlayCoinSFX()
    {
        if (sfxSource != null && coinSFX != null)
            sfxSource.PlayOneShot(coinSFX);
    }

    public void PlayScoreSFX()
    {
        if (sfxSource != null && scoreSFX != null)
            sfxSource.PlayOneShot(scoreSFX);
    }

    public void PlayHitSFX()
    {
        if (sfxSource != null && hitSFX != null)
            sfxSource.PlayOneShot(hitSFX);
    }

    /// <summary>
    /// Phát tiếng vỗ cánh/nhảy của Player
    /// </summary>
    public void PlayFlapSFX()
    {
        if (sfxSource != null && flapSFX != null)
            sfxSource.PlayOneShot(flapSFX);
    }
}