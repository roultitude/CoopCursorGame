using UnityEngine;
using System.Collections;

public class CurvedArcMover : MonoBehaviour
{

    [SerializeField]
    Vector2 dest = new Vector2(5f, 2f);
    [SerializeField]
    float curveAmount = 2f;
    [SerializeField]
    float duration = 1.5f;

    [ContextMenu("TestCurve")]
    private void TestCurve()
    {

        GetComponent<CurvedArcMover>().Launch(dest, curveAmount, duration);
    }
    public void Launch(Vector2 targetPosition, float curveAmount = 1.5f, float duration = 1f)
    {
        StartCoroutine(CurvedMovement(targetPosition, curveAmount, duration));
    }

    private IEnumerator CurvedMovement(Vector2 target, float curveAmount, float duration)
    {
        Vector2 start = transform.position;
        Vector2 controlPoint = (start + target) / 2 + new Vector2(0, curveAmount); // Curve peak

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration; // Normalized time (0 to 1)

            // Quadratic Bezier curve formula: (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
            Vector2 position =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * controlPoint +
                Mathf.Pow(t, 2) * target;

            transform.position = position;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = target; // Ensure it reaches exactly the target
    }
}
