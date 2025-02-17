using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveTowardsPlayer", story: "Moves [Agent] towards [player] position", category: "Action", id: "9e80bb9b55a64c54f7d60dc9096abee3")]
public partial class MoveTowardsPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Player> Player;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector2 dir = (Player.Value.transform.position - Agent.Value.transform.position).normalized;
        Agent.Value.transform.position += (Vector3) dir * Speed * Time.deltaTime;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

