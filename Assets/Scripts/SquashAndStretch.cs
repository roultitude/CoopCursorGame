using System;
using System.Collections;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [Header("Squash and Stretch Core")]
    [SerializeField, Tooltip("Default to current GO transform if unset")] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;

    [SerializeField, Range(0, 1f)] private float animDuration = 0.25f;

    [SerializeField] private bool canBeOverwritten;

    [SerializeField] private bool playOnStart;

    [Flags]
    enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }


    [Header("Animation Settings")]
    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maxScale = 1.3f;
    [SerializeField] private bool resetToInitialAfterAnim = true;
    [SerializeField]
    private AnimationCurve squashAndStretchCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
        );


    [Header("Looping Settings")]
    [SerializeField] private bool looping;
    [SerializeField] private float loopingDelay = 0.5f;

    private Coroutine squashAndStretchCoroutine;
    private WaitForSeconds loopingDelayWaitForSeconds;
    private Vector3 initialScaleVector;

    private bool isAffectingX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool isAffectingY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool isAffectingZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake()
    {
        if (!transformToAffect)
        {
            transformToAffect = transform;
        }
        initialScaleVector = transform.localScale;
        loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
    }

    public void Start()
    {
        if (playOnStart)
        {
            CheckForAndStartCoroutine();
        }
    }
    [ContextMenu("Play Squash and Stretch")]
    public void PlaySquashAndStretch()
    {
        if(looping && !canBeOverwritten)
        {
            return;
        }
        CheckForAndStartCoroutine();
    }
    private void CheckForAndStartCoroutine()
    {
        if(axisToAffect == SquashStretchAxis.None)
        {
            Debug.LogError("Axis to affect set to none", gameObject);
            return;
        }

        if (squashAndStretchCoroutine!=null)
        {
            StopCoroutine(squashAndStretchCoroutine);
            if (resetToInitialAfterAnim)
            {
                transform.localScale = initialScaleVector;
            }
        }

        squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            float elapsedTime = 0;
            Vector3 originalScale = initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < animDuration)
            {
                elapsedTime += Time.deltaTime;
                float curvePosition = elapsedTime / animDuration;
                float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maxScale - initialScale));

                float minimumThreshold = 0.0001f;
                if(Mathf.Abs(remappedValue) < minimumThreshold)
                {
                    remappedValue = minimumThreshold;
                }

                if (isAffectingX)
                {
                    modifiedScale.x = originalScale.x * remappedValue;
                } else
                {
                    modifiedScale.x = originalScale.x / remappedValue;
                }
                if (isAffectingY)
                {
                    modifiedScale.y = originalScale.y * remappedValue;
                }
                else
                {
                    modifiedScale.y = originalScale.y / remappedValue;
                }
                if (isAffectingZ)
                {
                    modifiedScale.z = originalScale.z * remappedValue;
                }
                else
                {
                    modifiedScale.z = originalScale.z / remappedValue;
                }
                transformToAffect.localScale = modifiedScale;
                yield return null;
            }

            if (resetToInitialAfterAnim)
            {
                transformToAffect.localScale = originalScale;
            }
            if (looping)
            {
                yield return loopingDelayWaitForSeconds;
            }
        } while (looping);
    }

    public void SetLooping(bool toLoop)
    {
        looping = toLoop;
    }




}
