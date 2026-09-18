using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float jumpForce = 5.5f; // Lực nảy theo GDD
    [SerializeField] private float maxUpwardAngle = 20.0f; // Góc ngửa lên
    [SerializeField] private float maxDownwardAngle = -70.0f; // Góc chúi xuống
    [SerializeField] private float tiltSmoothness = 5.0f; // Tốc độ mượt góc xoay

    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;

        // Nhận input Space hoặc Tap màn hình
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Flap();
        }

        UpdateRotation();
    }

    void Flap()
    {
        // Gán trực tiếp vận tốc trục Y để lực nảy tức thì, phản hồi chính xác
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void UpdateRotation()
    {
        // Tính toán góc xoay dựa trên vận tốc rơi/nảy
        float targetAngle = (rb.linearVelocity.y > 0) ? maxUpwardAngle : maxDownwardAngle;

        // Tạo hiệu ứng xoay mượt (Game Feel)
        float currentAngle = transform.eulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360; // Chuẩn hóa góc âm

        float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * tiltSmoothness);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Xử lý va chạm với vách ống hoặc mặt đất
        isDead = true;
        Debug.Log("Player Died!");
        // Viết sự kiện gọi GameManager sau này
    }
}