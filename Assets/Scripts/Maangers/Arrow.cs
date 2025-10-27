using UnityEngine;
using System.Collections;

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
        StartCoroutine(FadeAndDisable());
    }

    public void OnCorrectSwipe()
    {
        if (swiped) return;
        swiped = true;
        GameManager.Instance.AddScore(1);
        if (sr) sr.color = Color.green;
        StartCoroutine(FadeAndDisable());
    }

    public void OnWrongSwipe()
    {
        if (swiped) return;
        swiped = true;
        GameManager.Instance.GameOver();
        if (sr) sr.color = Color.red;
        StartCoroutine(FadeAndDisable());
    }

    IEnumerator FadeAndDisable()
    {
        isActive = false;
        float fadeTime = 0.3f;
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < fadeTime)
        {
            float t = elapsed / fadeTime;
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
