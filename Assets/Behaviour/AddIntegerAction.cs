using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Add Integer", story: "Add [Amount] to [Integer]", category: "Action", id: "cf20036b573c42b316ffdbc316652f32")]
public partial class AddIntegerAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Amount;
    [SerializeReference] public BlackboardVariable<int> Integer;

    protected override Status OnStart()
    {
        Integer.Value += Amount;
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

