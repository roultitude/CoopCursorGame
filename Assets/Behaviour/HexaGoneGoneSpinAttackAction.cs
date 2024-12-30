using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HexaGoneGoneSpinAttack", story: "HexaGoneGone flies upwards then spins in from left side", category: "Action", id: "53c45d7fddb95c7e280fb1e41c2c4164")]
public partial class HexaGoneGoneSpinAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Boss_HexaHexaGone> HexaGoneGone;
    [SerializeReference] public BlackboardVariable<float> LineIndicatorDelay;
    [SerializeReference] public BlackboardVariable<float> UpwardMovementDuration;
    [SerializeReference] public BlackboardVariable<float> UpwardMaxY;
    [SerializeReference] public BlackboardVariable<int> NumAttacks;

    private int numParts;
    private float timer;
    private AnimationCurve easeCurve;
    private int numCompletedAttacks;
    protected override Status OnStart()
    {
        numParts = HexaGoneGone.Value.partSpawnPos.Length;
        timer = 0;
        numCompletedAttacks = 0;
        easeCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(1f, 1f));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer < UpwardMovementDuration)
        {
            Agent.Value.transform.position = new Vector2(0, easeCurve.Evaluate(timer / UpwardMovementDuration) * UpwardMaxY); // fly upwards
            for (int i = 0; i < Mathf.Min(numParts, HexaGoneGone.Value.partPosition.Length); i++)
            {
                HexaGoneGone.Value.partPosition[i] =
                    HexaGoneGone.Value.partSpawnPos[i]
                    + (Vector2)Agent.Value.transform.position;
            }
            return Status.Running;
        }
        if(timer < UpwardMovementDuration+numCompletedAttacks*LineIndicatorDelay)
        {
            //wait for timer 
            for (int i = 0; i < Mathf.Min(numParts, HexaGoneGone.Value.partPosition.Length); i++)
            {
                HexaGoneGone.Value.partPosition[i] =
                    HexaGoneGone.Value.partSpawnPos[i]
                    + (Vector2)Agent.Value.transform.position;
            }
            return Status.Running;
        }
        if (numCompletedAttacks < NumAttacks)
        {
            //set indicator and prep attack
            HexaGoneGone.Value.SpinAttack(numCompletedAttacks, (numCompletedAttacks % 2) == 0, LineIndicatorDelay);
            numCompletedAttacks++;
            return Status.Running;
        }
        float totalDuration =  UpwardMovementDuration * 2 + numCompletedAttacks * LineIndicatorDelay;
        if (timer < totalDuration) //done with all attacks
        {
            //move back to origin
            Agent.Value.transform.position = new Vector2(0, easeCurve.Evaluate((totalDuration - timer) / UpwardMovementDuration) * UpwardMaxY); // fly upwards
            for (int i = 0; i < Mathf.Min(numParts, HexaGoneGone.Value.partPosition.Length); i++)
            {
                HexaGoneGone.Value.partPosition[i] =
                    HexaGoneGone.Value.partSpawnPos[i]
                    + (Vector2)Agent.Value.transform.position;
            }
            return Status.Running;

        }
        //done
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

