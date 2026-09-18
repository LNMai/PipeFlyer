using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private int poolSize = 5;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.8f;
    [SerializeField] private float minY = -2.0f;
    [SerializeField] private float maxY = 2.0f;

    private List<GameObject> pipePool = new List<GameObject>();
    private float timer = 0f;

    void Start()
    {
        // Khởi tạo danh sách các ống sẵn trong Pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(pipePrefab);
            obj.SetActive(false);
            pipePool.Add(obj);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    void SpawnPipe()
    {
        GameObject pipe = GetPooledPipe();
        if (pipe != null)
        {
            // Ngẫu nhiên hóa độ cao trục Y trong khoảng an toàn
            float randomY = Random.Range(minY, maxY);
            pipe.transform.position = new Vector3(transform.position.x, randomY, 0);
            pipe.SetActive(true);
        }
    }

    GameObject GetPooledPipe()
    {
        // Tìm ống đang ẩn để tái sử dụng
        foreach (GameObject pipe in pipePool)
        {
            if (!pipe.activeInHierarchy)
            {
                return pipe;
            }
        }
        return null;
    }
}