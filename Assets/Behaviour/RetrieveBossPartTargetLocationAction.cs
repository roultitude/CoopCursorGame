using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RetrieveBossPartTargetLocation", story: "Set [TargetLocation] using own [PartNumber] and BossParent in [MinionController]", category: "Action", id: "16bd984ce190bdb66779e8d9c9455635")]
public partial class RetrieveBossPartTargetLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector2> TargetLocation;
    [SerializeReference] public BlackboardVariable<int> PartNumber;
    [SerializeReference] public BlackboardVariable<BossMinionController> MinionController;

    protected override Status OnStart()
    {
        TargetLocation.Value = MinionController.Value.GetTargetPosition();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Failure; //should never fire.
        
    }

    protected override void OnEnd()
    {
    }
}

