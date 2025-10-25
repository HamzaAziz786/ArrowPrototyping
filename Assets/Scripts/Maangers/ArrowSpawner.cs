using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public ObjectPool arrowPool;
    public float spawnInterval = 2f;
    private float timer;

    private Vector2[] spawnDirs = new Vector2[]
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    private float currentArrowLife = 2f;
    private float spawnDistance = 6f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnArrow();
            timer = 0f;
        }
    }

    void SpawnArrow()
    {
        // pick random direction (up, down, left, right)
        Vector2 dir = spawnDirs[Random.Range(0, spawnDirs.Length)];
        Vector3 spawnPos = (Vector3)dir * spawnDistance;

        GameObject arrow = arrowPool.GetObject();
        arrow.transform.position = spawnPos;

        // Rotate arrow toward center
        Vector3 toCenter = (Vector3.zero - spawnPos).normalized;
        float angle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 270);

        arrow.GetComponent<Arrow>().Init(toCenter, currentArrowLife);
    }

    public void IncreaseDifficulty()
    {
        spawnInterval = Mathf.Max(0.4f, spawnInterval - 0.1f);
        currentArrowLife = Mathf.Max(0.6f, currentArrowLife - 0.1f);
    }
}
