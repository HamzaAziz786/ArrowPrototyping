using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public ObjectPool arrowPool;
    public float spawnInterval = 2f;
    private float timer;
    private float currentArrowLife = 2f;
    public float spawnDistance = 6f;

    // 8 direction vectors (4 sides + 4 diagonals)
    private Vector2[] spawnDirs = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(1, 1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, -1).normalized
    };

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
        // choose random direction
        Vector2 dir = spawnDirs[Random.Range(0, spawnDirs.Length)];
        Vector3 spawnPos = (Vector3)dir * spawnDistance;

        GameObject arrow = arrowPool.GetObject();
        arrow.transform.position = spawnPos;

        // arrow rotation → always face toward center
        Vector3 toCenter = (Vector3.zero - spawnPos).normalized;
        float angle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        arrow.GetComponent<Arrow>().Init(toCenter, currentArrowLife);
    }

    public void IncreaseDifficulty()
    {
        spawnInterval = Mathf.Max(0.4f, spawnInterval - 0.1f);
        currentArrowLife = Mathf.Max(0.6f, currentArrowLife - 0.1f);
    }
}
