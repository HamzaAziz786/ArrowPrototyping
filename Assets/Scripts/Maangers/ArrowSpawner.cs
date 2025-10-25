using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    [Header("Pool Reference")]
    public ObjectPool arrowPool;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnDistance = 6f;
    private float timer;

    [Header("Arrow Settings")]
    private float currentArrowLife = 4f;

    [Header("Difficulty Settings")]
    public float difficultyIncreaseInterval = 10f; // every 10 sec, increase difficulty
    public float spawnIntervalDecrease = 0.1f;     // faster spawns
    public float arrowLifeDecrease = 0.1f;         // shorter time to react
    public float minSpawnInterval = 0.4f;
    public float minArrowLife = 0.6f;

    private float difficultyTimer;

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
        // Spawn arrows
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnArrow();
            timer = 0f;
        }

        // Handle difficulty increase
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
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
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle - 270);

        arrow.GetComponent<Arrow>().Init(toCenter, currentArrowLife);
    }

    public void IncreaseDifficulty()
    {
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);
        currentArrowLife = Mathf.Max(minArrowLife, currentArrowLife - arrowLifeDecrease);

        Debug.Log($"Difficulty increased! SpawnInterval={spawnInterval:F2}, ArrowLife={currentArrowLife:F2}");
    }
}
