using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 0.2f;
    public Texture[] difficultyBackgrounds; // 0: Dễ, 1: Trung bình, 2: Khó

    private Renderer meshRenderer;
    private float currentOffset = 0f;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        // Đọc đúng key "DifficultyLevel" từ MainMenuManager
        int level = PlayerPrefs.GetInt("DifficultyLevel", 0);
        SetDifficultyBackground(level);
    }

    void Update()
    {
        // Dừng cuộn nền khi Game Over
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        currentOffset += Time.deltaTime * scrollSpeed;
        meshRenderer.material.mainTextureOffset = new Vector2(currentOffset, 0);
    }

    public void SetDifficultyBackground(int levelIndex)
    {
        if (difficultyBackgrounds != null && levelIndex >= 0 && levelIndex < difficultyBackgrounds.Length)
        {
            meshRenderer.material.mainTexture = difficultyBackgrounds[levelIndex];
        }

        // Chỉnh tốc độ nền tương ứng với độ khó
        if (levelIndex == 0) scrollSpeed = 0.15f;      // Dễ
        else if (levelIndex == 1) scrollSpeed = 0.25f; // Trung bình
        else if (levelIndex == 2) scrollSpeed = 0.4f;  // Khó
    }
}