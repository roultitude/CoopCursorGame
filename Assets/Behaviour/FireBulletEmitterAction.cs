using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using BulletPro;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyFireEmitter", story: "Enemy Sets [BulletEmitter] to [IsActive]", category: "Action", id: "ed10fb0cfecb2ef8e7c66bbd5fcb80d3")]
public partial class EnemyFireBulletEmitterAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<BulletEmitter> BulletEmitter;
    [SerializeReference] public BlackboardVariable<bool> IsActive = new BlackboardVariable<bool>(true);

    protected override Status OnStart()
    {
        Enemy.Value.TriggerEmitter(BulletEmitter.Value,IsActive);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        //should never run
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

