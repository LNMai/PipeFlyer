using UnityEngine;

/// <summary>
/// Điều khiển chuyển động, góc xoay, đổi skin và va chạm của Player.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float jumpForce = 5.5f;           // Lực nhảy lên
    [SerializeField] private float maxUpwardAngle = 20.0f;     // Góc nghiêng tối đa khi bay lên
    [SerializeField] private float maxDownwardAngle = -70.0f;  // Góc cắm đầu tối đa khi rơi xuống
    [SerializeField] private float tiltSmoothness = 5.0f;      // Tốc độ mượt của chuyển động xoay

    [Header("Skin Settings")]
    [SerializeField] private Sprite[] skinSprites; // Danh sách Sprite skin xếp đúng thứ tự với ShopManager

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Áp dụng Skin đã được chọn từ Shop khi bắt đầu
        ApplySelectedSkin();
    }

    private void Update()
    {
        if (isDead) return;

        // Nhận đầu vào nhảy từ bàn phím (Space) hoặc chuột trái / chạm màn hình
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Flap();
        }

        UpdateRotation();
    }

    /// <summary>
    /// Thực hiện cú nhảy và phát âm thanh vỗ cánh
    /// </summary>
    private void Flap()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFlapSFX();
        }
    }

    /// <summary>
    /// Cập nhật góc xoay của nhân vật dựa trên vận tốc rơi Y
    /// </summary>
    private void UpdateRotation()
    {
        float targetAngle = (rb.linearVelocity.y > 0) ? maxUpwardAngle : maxDownwardAngle;
        float currentAngle = transform.eulerAngles.z;

        // Chuyển đổi góc từ 0..360 sang -180..180 để tính toán Lerp chính xác
        if (currentAngle > 180) currentAngle -= 360;

        float newAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * tiltSmoothness);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    /// <summary>
    /// Áp dụng Sprite skin đã được người chơi chọn trong PlayerPrefs
    /// </summary>
    public void ApplySelectedSkin()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
        if (skinSprites != null && selectedIndex < skinSprites.Length && spriteRenderer != null)
        {
            spriteRenderer.sprite = skinSprites[selectedIndex];
        }
    }

    /// <summary>
    /// Xử lý va chạm vật lý với Ống hoặc Mặt đất -> Thua cuộc
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Tránh xử lý thua nhiều lần

        isDead = true;
        Debug.Log("[PlayerController] Va chạm vật thể: " + collision.gameObject.name);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    /// <summary>
    /// Đưa Player về vị trí xuất phát và reset trạng thái khi chơi lại
    /// </summary>
    public void ResetPlayer(Vector3 startPosition)
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        isDead = false;
    }
}