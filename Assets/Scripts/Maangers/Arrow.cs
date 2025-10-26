using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
    private Vector2 moveDir;
    private float moveSpeed = 3f;
    private float lifeTime;
    private float timer;
    private bool isFading = false;
    public bool swiped = false;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private SpriteRenderer childRenderer;

    private Coroutine fadeRoutine;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Init(Vector2 dir, float lifetime)
    {
        moveDir = dir;
        lifeTime = lifetime;
        timer = 0f;
        swiped = false;
        isFading = false;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (childRenderer != null)
        {
            Color c = childRenderer.color;
            c.a = 1f;
            childRenderer.color = c;
        }

        transform.localScale = originalScale;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isFading)
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer >= lifeTime && !swiped)
        {
            Missed();
        }
    }

    void Missed()
    {
        if (!swiped && !isFading)
        {
            swiped = true;
            GameManager.Instance.Miss();
            fadeRoutine = StartCoroutine(FadeAndShrinkDisable());
        }
    }

    public void Swipe(Vector2 swipeDir)
    {
        if (swiped || isFading) return;

        float angle = Vector2.Angle(swipeDir.normalized, -moveDir.normalized);

        if (angle < 90)
        {
            swiped = true;
            GameManager.Instance.Hit();
            gameObject.SetActive(false);
        }
        else
        {
            swiped = true;
            GameManager.Instance.Miss();
            fadeRoutine = StartCoroutine(FadeAndShrinkDisable());
        }
    }

    private IEnumerator FadeAndShrinkDisable()
    {
        if (childRenderer == null)
        {
            gameObject.SetActive(false);
            yield break;
        }

        isFading = true;

        float elapsed = 0f;
        Color startColor = childRenderer.color;
        Vector3 startScale = transform.localScale;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;

            // Fade out
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            childRenderer.color = c;

            // Shrink
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set fully transparent & scaled down
        Color final = childRenderer.color;
        final.a = 0f;
        childRenderer.color = final;

        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
        isFading = false;
    }
}
