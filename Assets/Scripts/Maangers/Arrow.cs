using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector2 moveDir;
    private float moveSpeed = 3f;
    private float lifeTime;
    private float timer;
    public bool swiped = false;

    public void Init(Vector2 dir, float lifetime)
    {
        moveDir = dir;
        lifeTime = lifetime;
        timer = 0f;
        swiped = false;
    }

    void Update()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Missed();
        }
    }

    void Missed()
    {
        if (!swiped)
        {
            GameManager.Instance.Miss();
        }
        gameObject.SetActive(false);
    }

    public void Swipe(Vector2 swipeDir)
    {
        if (swiped) return;

        float angle = Vector2.Angle(swipeDir.normalized, -moveDir.normalized);
        if (angle < 45f)
        {
            swiped = true;
            GameManager.Instance.Hit();
            gameObject.SetActive(false);
        }
        else
        {
            GameManager.Instance.Miss();
            gameObject.SetActive(false);
        }
    }
}
