using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyPlayAnimation", story: "Play [Enemy] animation [AnimState]", category: "Action", id: "7ade830d64841bd563dba34c773f286f")]
public partial class EnemyPlayAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<EnemyAnimationState> AnimState;
    protected override Status OnStart()
    {
        switch (AnimState.Value)
        {
            case EnemyAnimationState.idle:
                Enemy.Value.PlayIdleAnimationVisualRPC();
                break;
            case EnemyAnimationState.windup:
                Enemy.Value.PlayWindupAnimationVisualRPC();
                break;
            case EnemyAnimationState.attack:
                Enemy.Value.PlayAttackAnimationVisualRPC();
                break;
            case EnemyAnimationState.misc1:
                Enemy.Value.PlayMiscOneAnimationVisualRPC();
                break;
            case EnemyAnimationState.misc2:
                Enemy.Value.PlayMiscTwoAnimationVisualRPC();
                break;
            case EnemyAnimationState.jumping:
                Enemy.Value.PlayJumpingAnimationVisualRPC();
                break;
            default:
                return Status.Failure;
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

