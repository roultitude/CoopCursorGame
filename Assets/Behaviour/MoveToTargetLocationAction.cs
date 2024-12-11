using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToTargetLocation", story: "[Agent] moves to [Location]", category: "Action", id: "5de8886cbaf379d99eb53c57ed2b512b")]
public partial class MoveToTargetLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> Location;
    [SerializeReference] public BlackboardVariable<Enemy> EnemyScript;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<bool> RotateToFaceTarget = new BlackboardVariable<bool>(false);
    [SerializeReference] public BlackboardVariable<float> ArrivalThreshold = new BlackboardVariable<float>(0.0001f);
    [SerializeReference] public BlackboardVariable<float> LerpFraction = new BlackboardVariable<float>(0.8f);
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (PlayerManager.Instance.players.Count == 0) return Status.Failure;
        Vector2 displ = Location - (Vector2)Agent.Value.transform.position;
        Vector2 dir = displ.normalized;
        if (RotateToFaceTarget.Value)
        {
            EnemyScript.Value.Rotate(dir);
        }
        Vector2 movedVec = dir * Speed * Time.deltaTime;
        Vector3 targetPos;
        if (displ.sqrMagnitude < movedVec.sqrMagnitude)
        {
            targetPos = Location.Value;
        }
        else
        {
            targetPos = Agent.Value.transform.position + (Vector3)dir * Speed * Time.deltaTime;
        }
        Agent.Value.transform.position = Vector3.Lerp(
            Agent.Value.transform.position,
            targetPos, 
            LerpFraction);

        if (((Vector2)Agent.Value.transform.position - Location).sqrMagnitude < ArrivalThreshold)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

