using UnityEngine;
using System.Collections;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 startTouch;
    private Vector2 endTouch;
    [SerializeField] private float minSwipeDistance = 50f;

    [Header("Line Visual Settings")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private float lineLifetime = 0.5f;
    [SerializeField] private Color defaultColor = Color.yellow;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private Color missColor = Color.red;
    [SerializeField] private float worldDepth = 10f; // Distance from camera to draw line

    void Update()
    {
        // Detect swipe either via mouse or touch
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseSwipe();
#else
        HandleTouchSwipe();
#endif
    }

    // -------- Mouse swipe for editor/PC testing --------
    void HandleMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTouch = Input.mousePosition;
            DetectSwipe();
        }
    }

    // -------- Touch swipe for mobile --------
    void HandleTouchSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouch = touch.position;
                DetectSwipe();
            }
        }
    }

    // -------- Main Swipe Logic --------
    void DetectSwipe()
    {
        Vector2 swipe = endTouch - startTouch;
        if (swipe.magnitude < minSwipeDistance)
        {
            Debug.Log("❌ Swipe too short – ignored");
            return;
        }

        swipe.Normalize();

        Ray ray = Camera.main.ScreenPointToRay(startTouch);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(startTouch.x, startTouch.y, worldDepth));
        Vector3 worldEnd = Camera.main.ScreenToWorldPoint(new Vector3(endTouch.x, endTouch.y, worldDepth));

        Color lineColor = defaultColor;

        if (hit.collider != null)
        {
            Arrow arrow = hit.collider.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.Swipe(swipe);
                lineColor = arrow.swiped ? hitColor : missColor;
                Debug.Log($"Swipe on {arrow.name} | {(arrow.swiped ? "✅ HIT" : "❌ MISS")}");
            }
            else
            {
                Debug.Log($"❌ Object {hit.collider.name} has no Arrow script.");
                lineColor = missColor;
            }
        }
        else
        {
            Debug.Log("❌ No collider hit by swipe.");
            lineColor = missColor;
        }

        // Draw and fade swipe line (visible in Game View)
        StartCoroutine(DrawSwipeLine(worldStart, worldEnd, lineColor));
    }

    // -------- Visual line with smooth fade-out --------
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
        Color startColor = color;

        while (elapsed < lineLifetime)
        {
            float t = elapsed / lineLifetime;
            Color faded = startColor;
            faded.a = Mathf.Lerp(1f, 0f, t);
            lr.startColor = faded;
            lr.endColor = faded;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(lineObj);
    }
}
