using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.AppUI.Core;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomRoam", story: "[Agent] roams within an area of radius [RoamRadius] randomly", category: "Action", id: "37a2af6c51ad563d6052e9a6f1e444cb")]
public partial class RandomRoam : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> RoamRadius;
    [SerializeReference] public BlackboardVariable<float> ArrivalThreshold = new BlackboardVariable<float>(0.01f);

    private Vector2 roamTarget;
    protected override Status OnStart()
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        do {
            roamTarget = UnityEngine.Random.insideUnitCircle * RoamRadius + (Vector2)Agent.Value.transform.position; //find a way to stop them from roaming off map
        }
        while (roamTarget.x < bottomLeft.x || roamTarget.x > topRight.x
            || roamTarget.y > topRight.y || roamTarget.y < bottomLeft.y);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector2 roamDirection = (roamTarget - (Vector2) Agent.Value.transform.position).normalized;
        
        Agent.Value.transform.position += (Vector3) roamDirection * Speed * Time.deltaTime;
        if(((Vector2) Agent.Value.transform.position - roamTarget).sqrMagnitude < ArrivalThreshold)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

