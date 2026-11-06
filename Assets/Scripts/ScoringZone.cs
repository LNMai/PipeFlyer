using System;
using UnityEngine;

public class ScoringZone : MonoBehaviour
{
    // Event thông báo khi ghi điểm (Loại bỏ tight coupling với ScoreManager)
    public static event Action OnScored;
    public static event Action<Vector3> OnScoredAtPosition;

    [Header("Settings")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private int scoreValue = 1;

    private bool isScored;

    private void OnEnable()
    {
        // Reset trạng thái khi tái sử dụng từ Object Pool
        isScored = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isScored || !collision.CompareTag(targetTag)) return;

        isScored = true;

        // Bắn event để ScoreManager / FXManager / AudioManager lắng nghe
        OnScored?.Invoke();
        OnScoredAtPosition?.Invoke(transform.position);
    }
}