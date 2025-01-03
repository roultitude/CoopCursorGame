using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HexaGoneGoneStartPhaseTwo", story: "HexaGoneGone checks for and if successful starts phase two", category: "Action", id: "efa36e23135b0890b9008efa8ffa3dc4")]
public partial class HexaGoneGoneStartPhaseTwoAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss_HexaHexaGone> HexaGoneGone;
    protected override Status OnStart()
    {
        if (HexaGoneGone.Value.IsReadyForPhaseTwo())
        {
            HexaGoneGone.Value.StartPhaseTwo();
            return Status.Success;
        }
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

