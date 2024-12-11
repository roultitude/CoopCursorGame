using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HexaGoneGone_RadialTentacleAttack", story: "[HexaGoneGone] radial part attack", category: "Action", id: "d402fdfff72cb5fea69f4a4d70ca3525")]
public partial class HexaGoneGoneRadialTentacleAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Boss_HexaHexaGone> HexaGoneGone;
    [SerializeReference] public BlackboardVariable<float> TelegraphDuration;
    [SerializeReference] public BlackboardVariable<float> AttackDuration;

    private int partNum;
    private float returnDuration = 1f;
    private float attackPauseDuration = 3f;
    private float telegraphTimer;
    private float telegraphPauseTimer;
    private float attackTimer;
    private float attackPauseTimer;
    private float returnTimer;
    private int rotSeed;
    private AnimationCurve easeCurve;
    protected override Status OnStart()
    {
        partNum = HexaGoneGone.Value.partSpawnPos.Length;
        telegraphTimer = 0;
        telegraphPauseTimer = 0;
        attackTimer = 0;
        attackPauseTimer = 0;
        returnTimer = 0;
        rotSeed = UnityEngine.Random.Range(1, partNum);
        easeCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(1f, 1f));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (telegraphTimer < TelegraphDuration)
        {
            // withdraw parts close to body
            for (int i = 0; i < Mathf.Min(partNum,HexaGoneGone.Value.partPosition.Count); i++)
            {
                HexaGoneGone.Value.partPosition[i] = 
                    HexaGoneGone.Value.partSpawnPos[i] * (1- easeCurve.Evaluate(telegraphTimer/TelegraphDuration) * 0.7f)
                    + (Vector2)Agent.Value.transform.position;
            }
            telegraphTimer += Time.deltaTime;
            return Status.Running;

        } else if (telegraphPauseTimer < 1f) // pause 1s
        {
            for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Count); i++)
            {
                HexaGoneGone.Value.partPosition[i] = HexaGoneGone.Value.partSpawnPos[i] * (0.3f)
                    + (Vector2)Agent.Value.transform.position;
            }
            telegraphPauseTimer += Time.deltaTime;
            return Status.Running;
        }
        else if (attackTimer < AttackDuration)
        {
            // extend parts away
            for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Count); i++)
            {
                HexaGoneGone.Value.partPosition[i] = HexaGoneGone.Value.partSpawnPos[i] * (0.3f + easeCurve.Evaluate(attackTimer / AttackDuration) * 1.7f)
                    + (Vector2)Agent.Value.transform.position;
            }
            attackTimer += Time.deltaTime;
            return Status.Running;

        } else if(attackPauseTimer < attackPauseDuration)
        {
            for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Count); i++)
            {
                Vector2 target = HexaGoneGone.Value.partSpawnPos[i] * (2f);
                float rotAngleRad =easeCurve.Evaluate(attackPauseTimer/attackPauseDuration)*(360f / partNum * Mathf.Deg2Rad * rotSeed);
                Vector2 targetOffset = new Vector2(
                    target.x * Mathf.Cos(rotAngleRad) - target.y * Mathf.Sin(rotAngleRad),
                    target.x * Mathf.Sin(rotAngleRad) + target.y * Mathf.Cos(rotAngleRad)
                );

                HexaGoneGone.Value.partPosition[i] = targetOffset + (Vector2)Agent.Value.transform.position;
            }
            attackPauseTimer += Time.deltaTime;
            return Status.Running;
        } 
        else if (returnTimer < returnDuration)
        {
            // parts come back
            for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Count); i++)
            {
                Vector2 target = HexaGoneGone.Value.partSpawnPos[i] * (2 - easeCurve.Evaluate(returnTimer / AttackDuration) * 1f);
                float rotAngleRad = easeCurve.Evaluate(1 - returnTimer / returnDuration) * -(360f / partNum * Mathf.Deg2Rad * (partNum - rotSeed));
                Vector2 targetOffset = new Vector2(
                    target.x * Mathf.Cos(rotAngleRad) - target.y * Mathf.Sin(rotAngleRad),
                    target.x * Mathf.Sin(rotAngleRad) + target.y * Mathf.Cos(rotAngleRad)
                );

                HexaGoneGone.Value.partPosition[i] = HexaGoneGone.Value.partSpawnPos[i] * (2 - easeCurve.Evaluate(returnTimer / AttackDuration) * 1f)
                    + (Vector2)Agent.Value.transform.position;
            }
        }
        // reset to default position
        for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Count); i++)
        {
            HexaGoneGone.Value.partPosition[i] = HexaGoneGone.Value.partSpawnPos[i] * 1f + (Vector2)Agent.Value.transform.position;
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

