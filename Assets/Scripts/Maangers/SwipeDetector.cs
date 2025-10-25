using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 startTouch;
    private Vector2 endTouch;
    private float minSwipeDistance = 50f;

    void Update()
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

    void DetectSwipe()
    {
        Vector2 swipe = endTouch - startTouch;
        if (swipe.magnitude < minSwipeDistance) return;

        swipe.Normalize();

        Ray ray = Camera.main.ScreenPointToRay(startTouch);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider != null)
        {
            Arrow arrow = hit.collider.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.Swipe(swipe);
            }
        }
    }
}
