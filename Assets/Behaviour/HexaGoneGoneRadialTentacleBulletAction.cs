using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HexaGoneGoneRadialTentacleBullet", story: "[HexaGoneGone] radial bullet attack while spinning, half clockwise half anticlockwise", category: "Action", id: "6a8da2742c6f6e84db46e68d6de683a5")]
public partial class HexaGoneGoneRadialTentacleBulletAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Boss_HexaHexaGone> HexaGoneGone;
    [SerializeReference] public BlackboardVariable<float> AttackDuration;


    private int partNum;
    private float attackTimer;
    private int rotSeed;
    private AnimationCurve easeCurve;
    protected override Status OnStart()
    {
        partNum = HexaGoneGone.Value.partSpawnPos.Length;
        attackTimer = 0;
        rotSeed = UnityEngine.Random.Range(1, partNum);
        easeCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(1f, 1f));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if (attackTimer < AttackDuration / 2)
        {
            // spin parts
            for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Length); i++)
            {
                Vector2 target = HexaGoneGone.Value.partSpawnPos[i];
                float rotAngleRad = easeCurve.Evaluate(attackTimer / (AttackDuration / 2)) * (360f * Mathf.Deg2Rad);
                Vector2 targetOffset = new Vector2(
                    target.x * Mathf.Cos(rotAngleRad) - target.y * Mathf.Sin(rotAngleRad),
                    target.x * Mathf.Sin(rotAngleRad) + target.y * Mathf.Cos(rotAngleRad)
                );

                HexaGoneGone.Value.partPosition[i] = targetOffset
                    + (Vector2)Agent.Value.transform.position;
            }
            attackTimer += Time.deltaTime;
            return Status.Running;

        }
        else if (attackTimer < AttackDuration)
        {
            for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Length); i++)
            {
                Vector2 target = HexaGoneGone.Value.partSpawnPos[i];
                float rotAngleRad = (1 - easeCurve.Evaluate((attackTimer - (AttackDuration / 2)) / AttackDuration)) * (360f * Mathf.Deg2Rad);
                Vector2 targetOffset = new Vector2(
                    target.x * Mathf.Cos(rotAngleRad) - target.y * Mathf.Sin(rotAngleRad),
                    target.x * Mathf.Sin(rotAngleRad) + target.y * Mathf.Cos(rotAngleRad)
                );

                HexaGoneGone.Value.partPosition[i] = targetOffset + (Vector2)Agent.Value.transform.position;
            }
            attackTimer += Time.deltaTime;
            return Status.Running;
        }

        // reset to default position
        for (int i = 0; i < Mathf.Min(partNum, HexaGoneGone.Value.partPosition.Length); i++)
        {
            HexaGoneGone.Value.partPosition[i] = HexaGoneGone.Value.partSpawnPos[i] * 1f + (Vector2)Agent.Value.transform.position;
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
        Debug.Log("End Attack 1");
    }
}

