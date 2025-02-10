using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RotateEnemyTowards", story: "Rotate [Enemy] towards [Position]", category: "Action/Enemy", id: "58f7d89429d69202cc88387c1b70c5c9")]
public partial class RotateEnemyTowardsAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Vector2> Position;
    [SerializeReference] public BlackboardVariable<bool> IsInstant;
    [SerializeReference] public BlackboardVariable<float> ClampedAngle = new BlackboardVariable<float>(90f);
    [SerializeReference] public BlackboardVariable<float> FullRevolutionDuration = new BlackboardVariable<float>(0.5f);

    protected override Status OnStart()
    {
        if (IsInstant)
        {
            Enemy.Value.SnapRotation(Position.Value - (Vector2)Enemy.Value.transform.position, ClampedAngle);
        } else
        {
            Enemy.Value.Rotate(Position.Value - (Vector2)Enemy.Value.transform.position,ClampedAngle,FullRevolutionDuration);
        }
        
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

