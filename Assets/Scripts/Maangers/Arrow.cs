using DG.Tweening;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector2 direction { get; private set; }
    public bool swiped = false;

    public System.Action OnArrowEnded;   // ✅ Event for ArrowSpawner

    private float lifetime;
    private bool isActive = false;

    private SpriteRenderer sr;
    private Tween moveTween;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // ✅ Called from ArrowSpawner
    public void Activate(Vector2 dir, float life)
    {
        direction = dir.normalized;
        lifetime = life;
        swiped = false;
        isActive = true;

        // Set rotation
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        if (sr)
        {
            sr.enabled = true;
            sr.color = Color.white;
            sr.DOFade(1f, 0f);
        }

        transform.localScale = Vector3.one;
        gameObject.SetActive(true);

        MoveDownward();
    }

    private void MoveDownward()
    {
        float targetY = Camera.main
            .ViewportToWorldPoint(new Vector3(0, -0.1f, 10f)).y;

        moveTween = transform.DOMoveY(targetY, lifetime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (!swiped)
                    Missed();
            });
    }

    private void Missed()
    {
        if (swiped) return;
        swiped = true;

        SoundManager.Instance.PlaySwipeWrong();
        EndArrow(true);
    }

    // ✅ Only visual rotation override
    public void SetVisualDirection(Vector2 dir)
    {
        if (sr == null) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        direction = dir.normalized;
    }

    public void OnCorrectSwipe()
    {
        if (swiped) return;
        swiped = true;

        SoundManager.Instance.PlaySwipeCorrect();
        moveTween?.Kill();

        if (sr) sr.color = Color.green;

        GameManager.Instance.AddScore(1);

        EndArrow(false);
    }

    public void OnWrongSwipe()
    {
        if (swiped) return;
        swiped = true;

        SoundManager.Instance.PlaySwipeWrong();
        moveTween?.Kill();

        if (sr) sr.color = Color.red;

        EndArrow(true);
    }

    private void EndArrow(bool isWrong)
    {
        isActive = false;

        sr?.DOKill();
        transform.DOKill();

        float fadeTime = 0.35f;

        // Fade + scale out
        sr?.DOFade(0f, fadeTime);
        transform.DOScale(Vector3.zero, fadeTime)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                if(!isWrong)
                {
                    // ✅ Tell spawner immediately
                    OnArrowEnded?.Invoke();
                }
                else
                {
                    GameManager.Instance.GameOver();
                }
            });
    }
}
