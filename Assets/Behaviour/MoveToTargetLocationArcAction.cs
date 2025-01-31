using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToTargetLocationArc", story: "[Agent] moves to [TargetLocation] in an arc", category: "Action", id: "b2ce1c506c31df5035cd55617405782f")]
public partial class MoveToTargetLocationArcAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> TargetLocation;
    [SerializeReference] public BlackboardVariable<float> CurveAmount;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private Vector2 controlPoint;
    private Vector2 startPos;
    private float timeElapsed;
    protected override Status OnStart()
    {
        startPos = Agent.Value.transform.position;
        controlPoint = (startPos + TargetLocation) / 2 + new Vector2(0, CurveAmount); // Curve peak
        timeElapsed = 0;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (timeElapsed < Duration)
        {
            float t = timeElapsed / Duration; // Normalized time (0 to 1)

            // Quadratic Bezier curve formula: (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
            Vector2 position =
                Mathf.Pow(1 - t, 2) * startPos +
                2 * (1 - t) * t * controlPoint +
                Mathf.Pow(t, 2) * TargetLocation.Value;

            Agent.Value.transform.position = position;
            timeElapsed += Time.deltaTime;
            return Status.Running;
        }
        Agent.Value.transform.position = TargetLocation.Value; // Ensure it reaches exactly the target
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

