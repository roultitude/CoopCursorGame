using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Boss Control", story: "Checks if Boss is actively controlling via [BossMinionController]", category: "Action", id: "1cc216bbdbc10bc99e634226e8580e38")]
public partial class CheckBossControlAction : Action
{
    [SerializeReference] public BlackboardVariable<BossMinionController> BossMinionController;

    protected override Status OnStart()
    {
        if (BossMinionController.Value.isControlledByBoss)
            return Status.Success;
        else return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        //should never call this
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

