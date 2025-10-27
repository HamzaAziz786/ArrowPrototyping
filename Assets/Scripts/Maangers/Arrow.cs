using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector2 direction;       // The direction this arrow *represents*
    public bool swiped = false;     // Has the player already swiped this arrow?
    private float lifetime = 2f;
    private float timer;

    private SpriteRenderer sr;
    private bool isActive = false;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>(); // In case sprite is child
    }

    /// <summary>
    /// Initializes the arrow with its direction and lifetime.
    /// </summary>
    public void Activate(Vector2 dir, float life)
    {
        direction = dir.normalized;
        lifetime = life;
        timer = 0f;
        swiped = false;
        isActive = true;

        // Rotate to face direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // Reset visuals
        if (sr)
        {
            sr.enabled = true;
            sr.color = Color.white;
        }

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= lifetime && !swiped)
        {
            Missed();
        }
    }

    private void Missed()
    {
        if (swiped) return;
        swiped = true;
        GameManager.Instance.GameOver();
        if (sr) sr.color = Color.red;
        FadeAndDisable(true);
    }

    public void OnCorrectSwipe()
    {
        if (swiped) return;
        swiped = true;
        GameManager.Instance.AddScore(1);
        if (sr) sr.color = Color.green;
        FadeAndDisable(false);
    }

    public void OnWrongSwipe()
    {
        if (swiped) return;
        swiped = true;
        //GameManager.Instance.GameOver();
        if (sr) sr.color = Color.red;
        FadeAndDisable(true);
    }

    public void FadeAndDisable(bool IsGameOver)
    {
        if (sr == null) return;

        isActive = false;

        // Instantly kill any running tweens on this transform or sprite
        sr.DOKill();
        transform.DOKill();

        // Duration of fade
        float fadeTime = .6f;

        // Reset color alpha (just in case)
        Color startColor = sr.color;
        startColor.a = 1f;
        sr.color = startColor;

        // Fade and scale simultaneously
        sr.DOFade(0f, .5f)
            .SetUpdate(true); // <-- Important: works even when Time.timeScale = 0

        transform.DOScale(Vector3.zero, fadeTime)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                if (IsGameOver)
                {
                    GameManager.Instance.GameOver();
                }
            });
    }
}
