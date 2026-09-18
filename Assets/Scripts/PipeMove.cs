using UnityEngine;

public class PipeMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.0f; // Tốc độ di chuyển theo GDD
    private float deadZone = -10.0f; // Vị trí X để ẩn ống đi

    void Update()
    {
        // Di chuyển ống sang trái
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Trả về pool (deactivate) khi chạy qua khỏi màn hình bên trái
        if (transform.position.x < deadZone)
        {
            gameObject.SetActive(false);
        }
    }
}