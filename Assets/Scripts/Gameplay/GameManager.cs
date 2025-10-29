using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText, highscoreText , GameOverScore;
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
    private int currentScore = 0;
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
    public void AnimateScore(int targetScore, float duration, TMP_Text text , string AttachedText)
    {
        int tempScore = 0;

        DOTween.To(() => tempScore, x =>
        {
            tempScore = x;
            text.text = AttachedText + tempScore.ToString();
        },
        targetScore,
        duration
        ).SetEase(Ease.Linear);
    }
    public void GameOver()
    {
        IsGameOver = true;
        if(PlayerPrefs.GetInt("Highscore") < Score)
        {
            PlayerPrefs.SetInt("Highscore", Score);
            PlayerPrefs.Save();
        }
        if(PlayerPrefs.GetInt("Highscore") > Score)
        {
            highscoreText.transform.DOScale(1.3f , 1).SetLoops(2 , LoopType.Yoyo);
        }
        AnimateScore(PlayerPrefs.GetInt("Highscore"), .4f, highscoreText , "HighScore : ");
        AnimateScore(Score, .4f, GameOverScore , "Your Score : ");
        GameOverPanel.SetActive(true);
        OnGameOver?.Invoke();
    }
    IEnumerable ShowGameOverPanel()
    {
        yield return new WaitForSeconds(2f);
        GameOverPanel.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Home()
    {
        SceneManager.LoadScene(0);
    }
    private void UpdateUI()
    {
        scoreText.text = "Score: " + Score;
    }
}
