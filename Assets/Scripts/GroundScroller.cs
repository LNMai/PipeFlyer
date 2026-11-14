using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [Tooltip("Hệ số quy đổi từ tốc độ game sang tốc độ cuộn UV của mặt đất")]
    public float speedMultiplier = 0.1f;

    private Renderer meshRenderer;
    private float currentOffset = 0f;

    void Awake()
    {
        meshRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Dừng cuộn khi Game Over
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // Lấy tốc độ chuẩn từ GameManager (tự động tăng theo độ khó Dễ/TB/Khó)
        float baseSpeed = (GameManager.Instance != null) ? GameManager.Instance.gameSpeed : 3f;

        currentOffset += Time.deltaTime * (baseSpeed * speedMultiplier);
        meshRenderer.material.mainTextureOffset = new Vector2(currentOffset, 0);
    }
}