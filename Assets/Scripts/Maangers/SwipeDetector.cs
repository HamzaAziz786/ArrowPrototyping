using UnityEngine;
using System.Collections;
using System.Linq;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 startTouch, endTouch;
    [SerializeField] private float minSwipeDistance = 50f;

    [Header("Line Visual Settings")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private float lineLifetime = 0.5f;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private Color missColor = Color.red;
    [SerializeField] private float worldDepth = 10f;

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseSwipe();
#else
        HandleTouchSwipe();
#endif
    }

    void HandleMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
            startTouch = Input.mousePosition;
        else if (Input.GetMouseButtonUp(0))
        {
            endTouch = Input.mousePosition;
            DetectSwipe();
        }
    }

    void HandleTouchSwipe()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
            startTouch = touch.position;
        else if (touch.phase == TouchPhase.Ended)
        {
            endTouch = touch.position;
            DetectSwipe();
        }
    }

    void DetectSwipe()
    {
        Vector2 swipe = endTouch - startTouch;
        if (swipe.magnitude < minSwipeDistance) return;
        swipe.Normalize();

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(startTouch.x, startTouch.y, worldDepth));
        Vector3 worldEnd = Camera.main.ScreenToWorldPoint(new Vector3(endTouch.x, endTouch.y, worldDepth));

        Arrow[] arrows = FindObjectsOfType<Arrow>().Where(a => a.gameObject.activeInHierarchy).ToArray();
        if (arrows.Length == 0) return;

        Arrow closest = arrows.OrderBy(a => Vector2.Distance(a.transform.position, worldStart)).FirstOrDefault();
        if (closest == null) return;

        Vector2 arrowDir = closest.direction;
        float dot = Vector2.Dot(arrowDir, -swipe);
        bool isOpposite = dot < -0.75f;

        if (isOpposite)
            closest.OnCorrectSwipe();
        else
            closest.OnWrongSwipe();

        StartCoroutine(DrawSwipeLine(worldStart, worldEnd, isOpposite ? hitColor : missColor));
    }

    IEnumerator DrawSwipeLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject lineObj = new GameObject("SwipeLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        float elapsed = 0f;
        while (elapsed < lineLifetime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / lineLifetime);
            lr.startColor = new Color(color.r, color.g, color.b, alpha);
            lr.endColor = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(lineObj);
    }
}
