using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RetrieveBossPartTargetLocation", story: "Set [TargetLocation] using own [PartNumber] and BossParent in [EnemyScript]", category: "Action", id: "16bd984ce190bdb66779e8d9c9455635")]
public partial class RetrieveBossPartTargetLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector2> TargetLocation;
    [SerializeReference] public BlackboardVariable<int> PartNumber;
    [SerializeReference] public BlackboardVariable<Enemy> EnemyScript;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!EnemyScript.Value.bossParent)
        {
            return Status.Failure;
        }
        if (EnemyScript.Value.bossParent.partPosition.Count < PartNumber) 
        {
            return Status.Failure;
        }
        TargetLocation.Value = EnemyScript.Value.bossParent.partPosition[PartNumber.Value];
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

