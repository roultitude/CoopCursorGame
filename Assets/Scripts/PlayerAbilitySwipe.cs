using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAbilitySwipe : NetworkBehaviour
{
    public NetworkVariable<bool> isAbilityAvailable = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    [Header("Swipe Settings")]
    public float swipeSpeedThreshold = 5f;  // Minimum speed for a swipe to register
    public float swipeDamage = 10f;         // Damage dealt by the swipe
    public float maxSwipeTrailLength = 0.5f; // Duration to store swipe path for detecting intersections
    public float maxAngleDeviation = 15f;   // Maximum allowed angle deviation (in degrees) for a swipe to be considered straight
    public float maxSwipeDuration = 0.5f;
    public float swipeFadeTime = 1f;
    public float swipeCooldown = 5f;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private AnimationCurve swipeVisualCurve;


    private List<Vector2> swipeTrail = new List<Vector2>(); // To store recent positions
    private Vector2 lastPosition;
    private float swipeCooldownTimer = 0;
    private float fadeTimer = 0;
    private bool isSwiping = false;
    private Color color;
    private Gradient lineRendererGradient = new Gradient();
    

    public void StartSwipe()
    {
        if (!IsOwner) return;
        swipeTrail.Clear();
        lastPosition = transform.position;
        lineRenderer.positionCount = 0;
        isSwiping = true;
        UpdateLineRendererAlpha(1);
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Q) && swipeCooldownTimer > swipeCooldown)
        {
            StartSwipe();
            Debug.Log("Swipe on");
            Invoke(nameof(EndSwipe), maxSwipeDuration);
        }
    }

    public override void OnNetworkSpawn()
    {
        Random.InitState((int)OwnerClientId);
        color = Random.ColorHSV();
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        swipeCooldownTimer = swipeCooldown;
    }
    private void FixedUpdate()
    {
        //tick timers
        fadeTimer += Time.fixedDeltaTime;
        swipeCooldownTimer += Time.fixedDeltaTime;

        if (IsOwner && swipeCooldownTimer > swipeCooldown)
        {
            isAbilityAvailable.Value = true;
        }

        if (!isSwiping) { //if not currently swiping fade visual
            UpdateLineRendererAlpha(1 - fadeTimer / swipeFadeTime);
            return;
        }

        TrackSwipePath();
        UpdateLineRenderer();

        if (!IsSwipeStraight())
        {
            Debug.Log("Swipe off, not straight");
            EndSwipe();
        }
    }

    private void EndSwipe()
    {
        if(!isSwiping) return;
        isSwiping = false;
        isAbilityAvailable.Value = false;
        DamageEnemiesInSwipePath();
        SyncVisualsRPC(swipeTrail.ToArray());
        fadeTimer = 0;
        swipeCooldownTimer = 0;

    }
    private void TrackSwipePath()
    {
        Vector2 currentPosition = transform.position;
        float distanceMoved = Vector2.Distance(currentPosition, lastPosition);

        // If player is moving fast enough, record position
        if (distanceMoved >= swipeSpeedThreshold * Time.deltaTime)
        {
            swipeTrail.Add(currentPosition);

            // Remove old points to limit trail length
            if (swipeTrail.Count > 0 && Vector2.Distance(swipeTrail[0], currentPosition) > maxSwipeTrailLength)
            {
                Debug.Log("Swipe off, length max");
                EndSwipe();
            }
        }

        lastPosition = currentPosition;
    }

    void UpdateLineRendererAlpha(float fadeProgress)
    {
        lineRendererGradient.SetKeys
        (
            lineRenderer.colorGradient.colorKeys,
            new GradientAlphaKey[] { new GradientAlphaKey(Mathf.Clamp(fadeProgress,0,1), 1f) }
        );
        lineRenderer.colorGradient = lineRendererGradient;
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = swipeTrail.Count;
        for (int i = 0; i < swipeTrail.Count; i++)
        {
            lineRenderer.SetPosition(i, swipeTrail[i]);
        }
        lineRenderer.widthCurve = swipeVisualCurve;

    }

    [Rpc(SendTo.NotMe)] //is this stupid???? is it supposed to go ServerRPC -> clientRPC???
    private void SyncVisualsRPC(Vector2[] trail)
    {
        lineRenderer.positionCount = trail.Length;
        for (int i = 0; i < trail.Length; i++)
        {
            lineRenderer.SetPosition(i, trail[i]);
        }
        lineRenderer.widthCurve = swipeVisualCurve;
        fadeTimer = 0;
        UpdateLineRendererAlpha(1);
    }

    bool IsSwipeStraight()
    {
        if (swipeTrail.Count < 3) return true; // A straight line requires at least two segments

        // Calculate the reference direction using the first segment
        Vector2 referenceDirection = (swipeTrail[1] - swipeTrail[0]).normalized;

        for (int i = 1; i < swipeTrail.Count - 1; i++)
        {
            Vector2 currentDirection = (swipeTrail[i + 1] - swipeTrail[i]).normalized;
            float angle = Vector2.Angle(referenceDirection, currentDirection);

            // If the angle exceeds the max allowed deviation, consider the swipe not straight
            if (angle > maxAngleDeviation)
            {
                return false;
            }
        }
        return true;
    }

    void DamageEnemiesInSwipePath()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, maxSwipeTrailLength))
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();
            if (enemy)
            {
                Debug.Log("FOund enemy");
                if (IsEnemyInSwipePath(collider))
                {
                    Debug.Log("Enemy in path");
                    enemy.OnDamage(swipeDamage);
                }
            }
        }
    }

    // Determine if an enemy intersects with the swipe path
    bool IsEnemyInSwipePath(Collider2D enemyCollider)
    {
        for (int i = 0; i < swipeTrail.Count - 1; i++)
        {
            Vector2 startPoint = swipeTrail[i];
            Vector2 endPoint = swipeTrail[i + 1];

            if (LineIntersectsCircle(startPoint, endPoint, enemyCollider.transform.position, enemyCollider.bounds.extents.x))
            {
                return true;
            }
        }
        return false;
    }

    bool LineIntersectsCircle(Vector2 lineStart, Vector2 lineEnd, Vector2 circleCenter, float circleRadius)
    {
        Vector2 closestPoint = ClosestPointOnLine(lineStart, lineEnd, circleCenter);
        float distanceToCircle = Vector2.Distance(closestPoint, circleCenter);
        return distanceToCircle <= circleRadius;
    }

    // Find the closest point on a line segment to a given point
    Vector2 ClosestPointOnLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        Vector2 lineDirection = lineEnd - lineStart;
        float length = lineDirection.magnitude;
        lineDirection.Normalize();

        float projection = Vector2.Dot(point - lineStart, lineDirection);
        projection = Mathf.Clamp(projection, 0, length);

        return lineStart + lineDirection * projection;
    }
}
