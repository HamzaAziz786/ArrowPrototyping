using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text scoreText;

    private int score;

    void Awake() => Instance = this;

    void Start()
    {
        score = 0;
        UpdateUI();
    }

    public void Hit()
    {
        score++;
        UpdateUI();
    }

    public void Miss()
    {
        score = Mathf.Max(0, score - 1);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText)
            scoreText.text = "Score: " + score;
    }
}
