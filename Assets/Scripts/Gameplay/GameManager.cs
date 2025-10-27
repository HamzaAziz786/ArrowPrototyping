using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject GameOverPanel;

    [Header("Game Settings")]
    [SerializeField] private float difficultyIncreaseRate = 5f; // Every 5 seconds
    [SerializeField] private float spawnRateMultiplier = 0.9f; // Arrows spawn faster

    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

    public delegate void GameEvent();
    public event GameEvent OnGameOver;
    public event GameEvent OnRestart;

    private float difficultyTimer;

    void Awake()
    {
        Instance = this;
        IsGameOver = false;
        Score = 0;
        UpdateUI();
        GameOverPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (IsGameOver) return;

        // Increase difficulty gradually
        //difficultyTimer += Time.deltaTime;
        //if (difficultyTimer >= difficultyIncreaseRate)
        //{
        //    difficultyTimer = 0f;
        //    ArrowSpawner.Instance.IncreaseDifficulty(spawnRateMultiplier);
        //}
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        Score += amount;
        UpdateUI();
    }

    public void GameOver()
    {
        
        IsGameOver = true;
       StartCoroutine(nameof(ShowGameOverPanel));
        OnGameOver?.Invoke();
        Time.timeScale = 0;
    }

    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSecondsRealtime(2f);
        GameOverPanel.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        Score = 0;
        IsGameOver = false;
        UpdateUI();
        GameOverPanel.gameObject.SetActive(false);
        OnRestart?.Invoke();
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + Score;
    }
}
