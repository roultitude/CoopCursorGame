using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemySetContactDamageMode", story: "Sets [EnemyContactDamageHandler] contact damage mode to [isActive]", category: "Action", id: "9c3521bbd212f7cb7397c3b2a6eefb4b")]
public partial class EnemySetContactDamageModeAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyContactDamageHandler> EnemyContactDamageHandler;
    [SerializeReference] public BlackboardVariable<bool> IsActive;

    protected override Status OnStart()
    {
        EnemyContactDamageHandler.Value.SetContactDamageState(IsActive);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

