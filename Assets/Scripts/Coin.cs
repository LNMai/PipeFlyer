using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool isCollected = false; // Cờ kiểm tra đã ăn chưa

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra đúng Tag Player và chưa từng ăn coin này
        if (!isCollected && collision.CompareTag("Player"))
        {
            isCollected = true; // Đánh dấu đã ăn ngay lập tức

            // Ẩn Sprite và tắt Collider để tránh va chạm phụ
            GetComponent<Collider2D>().enabled = false;
            if (TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.enabled = false;
            }

            // Cộng xu
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoins(1);
            }

            // Hủy GameObject Coin
            Destroy(gameObject);
        }
    }
}