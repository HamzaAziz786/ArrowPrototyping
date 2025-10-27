using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ArrowSpawner : MonoBehaviour
{
    [Header("Pool Reference")]
    public ObjectPool arrowPool;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    private float timer;

    [Header("Arrow Settings")]
    private float currentArrowLife = 2f;

    [Header("Difficulty Settings")]
    public float difficultyIncreaseInterval = 10f; // every 10 sec, increase difficulty
    public float spawnIntervalDecrease = 0.1f;     // faster spawns
    public float arrowLifeDecrease = 0.1f;         // shorter time to react
    public float minSpawnInterval = 0.6f;
    public float minArrowLife = 0.8f;

    private float difficultyTimer;
    private bool isGameOver = false;
    private GameObject currentArrow;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    // 8 possible directions
    private Vector2[] directions = new Vector2[]
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

    private void OnEnable()
    {
        gameManager.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        gameManager.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        isGameOver = true;
        if (currentArrow != null)
            currentArrow.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnArrow();
            timer = 0f;
        }

        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }

    void SpawnArrow()
    {
        // ensure only one arrow active at a time
        if (currentArrow != null && currentArrow.activeInHierarchy)
            return;

        // pick random direction
        Vector2 dir = directions[Random.Range(0, directions.Length)];

        // get arrow from pool
        currentArrow = arrowPool.GetObject();
        currentArrow.transform.position = Vector3.zero;

        // initialize arrow
        Arrow arrow = currentArrow.GetComponent<Arrow>();
        currentArrow.transform.DOScale(0, 0);
        currentArrow.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        arrow.Activate(dir, currentArrowLife);
    }

    void IncreaseDifficulty()
    {
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);
        currentArrowLife = Mathf.Max(minArrowLife, currentArrowLife - arrowLifeDecrease);
    }
}
