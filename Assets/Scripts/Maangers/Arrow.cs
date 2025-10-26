using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
    private Vector2 moveDir;
    private float moveSpeed = 3f;
    private float lifeTime;
    private float timer;
    private bool isFading = false; // track fade
    public bool swiped = false;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private SpriteRenderer childRenderer;

    private Coroutine fadeRoutine;

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

        gameObject.SetActive(true);
    }

    void Update()
    {
        // Stop movement if fading
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
            fadeRoutine = StartCoroutine(FadeAndDisable());
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
            fadeRoutine = StartCoroutine(FadeAndDisable());
        }
    }

    private IEnumerator FadeAndDisable()
    {
        if (childRenderer == null)
        {
            gameObject.SetActive(false);
            yield break;
        }

        isFading = true; // prevent movement or multiple fades

        float elapsed = 0f;
        Color startColor = childRenderer.color;

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            childRenderer.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Color final = childRenderer.color;
        final.a = 0f;
        childRenderer.color = final;

        gameObject.SetActive(false);
        isFading = false;
    }
}
