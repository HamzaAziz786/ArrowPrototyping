using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text scoreText;
    public event Action OnGameOver;
    private int score;
    public GameObject GameOverPanel;
    [SerializeField] private int MissScore, Threshold = 0;
    void Awake() => Instance = this;

    void Start()
    {
        score = 0;
        UpdateUI();
        MissScore = Threshold;
    }

    public void Hit()
    {
        score++;
        UpdateUI();
    }

    public void Miss()
    {
        MissScore--;
        if(MissScore <= 0)
            GameOver();
        score = Mathf.Max(0, score - 1);
        UpdateUI();
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        GameOverPanel.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void UpdateUI()
    {
        if (scoreText)
            scoreText.text = "Score: " + score;
    }
}
