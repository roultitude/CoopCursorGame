using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using BulletPro;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyFireBulletEmitter", story: "[Enemy] Sets [Emitter] to [IsActive]", category: "Action/Enemy", id: "ed10fb0cfecb2ef8e7c66bbd5fcb80d3")]
public partial class EnemyFireBulletEmitterAction : Action
{
    [SerializeReference] public BlackboardVariable<BulletEmitter> Emitter;
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<bool> IsActive = new BlackboardVariable<bool>(true);

    protected override Status OnStart()
    {
        Enemy.Value.TriggerEmitter(Emitter.Value,IsActive);
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

