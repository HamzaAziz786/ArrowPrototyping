using DG.Tweening;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Vector2 direction { get; private set; }  // now property
    public bool swiped = false;
    private float lifetime;
    private bool isActive = false;

    private SpriteRenderer sr;
    private Tween moveTween;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void Activate(Vector2 dir, float life)
    {
        direction = dir.normalized;
        lifetime = life;
        swiped = false;
        isActive = true;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        if (sr)
        {
            sr.enabled = true;
            sr.color = Color.white;
        }

        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
        MoveDownward();
    }

    void MoveDownward()
    {
        float targetY = Camera.main.ViewportToWorldPoint(new Vector3(0, -0.1f, 10f)).y;

        moveTween = transform.DOMoveY(targetY, lifetime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (!swiped)
                {
                    Missed();
                }
            });
    }

    private void Missed()
    {
        if (swiped) return;
        swiped = true;
        FadeAndDisable(true);
    }
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
        if (moveTween != null) moveTween.Kill();
        GameManager.Instance.AddScore(1);
        if (sr) sr.color = Color.green;
        FadeAndDisable(false);
    }

    public void OnWrongSwipe()
    {
        if (swiped) return;
        swiped = true;
        if (moveTween != null) moveTween.Kill();
        if (sr) sr.color = Color.red;
       
        FadeAndDisable(true);
    }

    public void FadeAndDisable(bool IsGameOver)
    {
        if (sr == null) return;
        isActive = false;
        sr.DOKill();
        transform.DOKill();

        float fadeTime = 0.5f;
        sr.DOFade(0f, fadeTime);
        transform.DOScale(Vector3.zero, fadeTime)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                if(IsGameOver)
                    GameManager.Instance.GameOver();
            });
    }
}
