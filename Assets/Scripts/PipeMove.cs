using UnityEngine;

public class PipeMove : MonoBehaviour
{
    [Header("Oscillation Settings (Dùng cho chế độ Khó)")]
    [SerializeField] private bool canMoveUpDown = false;
    [SerializeField] private float moveSpeedY = 1.2f;
    [SerializeField] private float moveDistanceY = 1.0f;

    private float startY;
    private int directionY = 1;

    private void Start()
    {
        startY = transform.position.y;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // Di chuyển sang trái theo tốc độ màn chơi
        float speed = (GameManager.Instance != null) ? GameManager.Instance.gameSpeed : 3.0f;
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // Nếu là cột màn Khó thì di chuyển lên xuống
        if (canMoveUpDown)
        {
            transform.Translate(Vector3.up * directionY * moveSpeedY * Time.deltaTime, Space.World);
            if (transform.position.y > startY + moveDistanceY) directionY = -1;
            else if (transform.position.y < startY - moveDistanceY) directionY = 1;
        }

        // Tự hủy khi ra khỏi màn hình
        if (transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }
}