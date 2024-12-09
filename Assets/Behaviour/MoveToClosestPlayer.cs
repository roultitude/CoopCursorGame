using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.AppUI.Core;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToClosestPlayer", story: "Moves [Agent] to closest Player", category: "Action", id: "61e6d54f7ee556b234e2983d9724d08f")]
public partial class MoveToClosestPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Enemy> EnemyScript;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
    [SerializeReference] public BlackboardVariable<bool> RotateToFacePlayer = new BlackboardVariable<bool>(false);

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (PlayerManager.Instance.players.Count == 0) return Status.Failure;
        Vector2 dir;
        Vector2 vecToPlayer = FindClosestPlayer().vecToPlayer;
        dir = vecToPlayer.normalized;
        if (RotateToFacePlayer.Value)
        {
            EnemyScript.Value.Rotate(dir);
        }
        Agent.Value.transform.position += (Vector3)dir * Speed * Time.deltaTime;

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }

    private (Player closestPlayer, Vector2 vecToPlayer) FindClosestPlayer()
    {
        Player closestPlayer = null;
        Vector2 closestVec = Vector2.zero;
        foreach (Player player in PlayerManager.Instance.players)
        {
            if (player.health.Value == 0) continue;
            if (!closestPlayer) // if first alive player
            {
                closestPlayer = player;
                closestVec = player.transform.position - Agent.Value.transform.position;
            }
            else //check against current closest player
            {
                Vector2 relPos = (player.transform.position - Agent.Value.transform.position);
                if (relPos.sqrMagnitude < closestVec.sqrMagnitude) //new closest
                {
                    closestPlayer = player;
                    closestVec = relPos;
                }
            }
        }

        if (!closestPlayer) // if all players dead
        {
            //Debug.Log("enemy cannot find alive player");
        }
        return (closestPlayer, closestVec);
    }
}

