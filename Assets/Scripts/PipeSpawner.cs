using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [Header("Prefabs theo độ khó")]
    [SerializeField] private GameObject pipeEasyPrefab;
    [SerializeField] private GameObject pipeMediumPrefab;
    [SerializeField] private GameObject pipeHardPrefab;

    [Header("Coin Prefab")]
    [SerializeField] private GameObject coinPrefab; // Thêm tham chiếu đến Prefab Coin
    [Range(0f, 1f)]
    [SerializeField] private float coinSpawnChance = 0.5f; // Tỉ lệ xuất hiện Coin (50%)

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRate = 2.0f;
    [SerializeField] private float heightOffset = 1.5f;

    private float timer = 0f;
    private int currentDifficulty = 0;

    public void SetDifficulty(int level)
    {
        currentDifficulty = level;
    }

    public void ResetSpawner()
    {
        timer = 0f;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (timer > spawnRate)
        {
            SpawnPipe();
            timer = 0;
        }
        timer += Time.deltaTime;
    }

    private void SpawnPipe()
    {
        // 1. Chọn loại ống theo độ khó
        GameObject prefabToSpawn = pipeEasyPrefab;
        if (currentDifficulty == 1 && pipeMediumPrefab != null) prefabToSpawn = pipeMediumPrefab;
        else if (currentDifficulty == 2 && pipeHardPrefab != null) prefabToSpawn = pipeHardPrefab;

        if (prefabToSpawn == null) return;

        // 2. Tính toán vị trí Y ngẫu nhiên cho ống
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;
        float randomY = Random.Range(lowestPoint, highestPoint);

        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);
        GameObject newPipePair = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);

        // 3. Sinh Coin ngẫu nhiên giữa khe ống (nếu có coinPrefab)
        if (coinPrefab != null && Random.value <= coinSpawnChance)
        {
            // Vị trí coin nằm chính giữa khe ống (tọa độ Y bằng randomY)
            Vector3 coinPos = new Vector3(transform.position.x, randomY, 0);
            GameObject newCoin = Instantiate(coinPrefab, coinPos, Quaternion.identity, newPipePair.transform);
        }
    }
}