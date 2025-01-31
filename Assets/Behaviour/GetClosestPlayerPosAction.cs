using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetClosestPlayerPos", story: "Retrieve closest player position to [Agent] and store in [Position]", category: "Action", id: "ea2a593382fdd21a34a82b3176a049ff")]
public partial class GetClosestPlayerPosAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector2> Position;

    protected override Status OnStart()
    {
        if (PlayerManager.Instance.players.Count == 0) return Status.Failure;
        Position.Value = FindClosestPlayer().closestPlayer.transform.position;
        return Status.Success;
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

