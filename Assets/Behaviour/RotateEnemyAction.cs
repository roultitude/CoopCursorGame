using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.UIElements;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RotateEnemy", story: "Rotate [Enemy] towards [Angle]", category: "Action/Enemy", id: "9fc83d2897aad022150238fe223ba050")]
public partial class RotateEnemyAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<float> Angle;
    [SerializeReference] public BlackboardVariable<bool> IsInstant;
    [SerializeReference] public BlackboardVariable<float> ClampedAngle = new BlackboardVariable<float>(90f);
    [SerializeReference] public BlackboardVariable<float> FullRevolutionDuration = new BlackboardVariable<float>(0.5f);

    protected override Status OnStart()
    {
        float rad = Angle * Mathf.Deg2Rad;
        if (IsInstant)
        {
            Enemy.Value.SnapRotation(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)), ClampedAngle);
        }
        else
        {
            Enemy.Value.Rotate(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)), ClampedAngle, FullRevolutionDuration);
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

