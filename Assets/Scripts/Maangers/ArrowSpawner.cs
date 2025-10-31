using UnityEngine;
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
    public float difficultyIncreaseInterval = 10f;
    public float spawnIntervalDecrease = 0.1f;
    public float arrowLifeDecrease = 0.1f;
    public float minSpawnInterval = 0.6f;
    public float minArrowLife = 0.8f;

    private float difficultyTimer;
    private bool isGameOver = false;
    private GameObject currentArrow;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private Camera cam;
    public Sprite[] ArrowSprites;

    // 8 visual arrow directions
    private readonly Vector2[] directions = new Vector2[]
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

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnEnable() => gameManager.OnGameOver += HandleGameOver;
    private void OnDisable() => gameManager.OnGameOver -= HandleGameOver;

    private void HandleGameOver()
    {
        isGameOver = true;
        if (currentArrow != null)
            currentArrow.SetActive(false);
    }

    private void Update()
    {
        if (isGameOver) return;

        // If no active arrow, fallback spawn via interval
        if (currentArrow == null || !currentArrow.activeInHierarchy)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnArrow();
                timer = 0f;
            }
        }

        // Difficulty timer
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }

    void SpawnArrow()
    {
        if (isGameOver) return;

        float randomX = Random.Range(0.5f, 0.5f);
        Vector3 spawnPos = cam.ViewportToWorldPoint(new Vector3(randomX, 1.15f, 10f));

        Vector2 randomDir = directions[Random.Range(0, directions.Length)];

        currentArrow = arrowPool.GetObject();
        currentArrow.transform.position = spawnPos;
        currentArrow.transform.localScale = Vector3.zero;

        float angle = Mathf.Atan2(randomDir.y, randomDir.x) * Mathf.Rad2Deg - 90f;
        currentArrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        currentArrow.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        Arrow arrow = currentArrow.GetComponent<Arrow>();
        arrow.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite =
            ArrowSprites[PlayerPrefs.GetInt("SelectedArrow")];

        // Hook immediate-spawn event
        arrow.OnArrowEnded = OnArrowEnded;

        // Arrow always moves downward
        arrow.Activate(Vector2.down, currentArrowLife);
        arrow.SetVisualDirection(randomDir);
    }

    private void OnArrowEnded()
    {
        if (!isGameOver)
        {
            // ✅ Instant spawn (NO WAIT)
            SpawnArrow();
        }
    }

    private void IncreaseDifficulty()
    {
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrease);
        currentArrowLife = Mathf.Max(minArrowLife, currentArrowLife - arrowLifeDecrease);
    }
}
