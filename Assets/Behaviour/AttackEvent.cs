using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AttackEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AttackEvent", message: "[Enemy] is beginning attack [id]", category: "Events", id: "5b352395931efba07abb5b486477efe0")]
public partial class AttackEvent : EventChannelBase
{
    public delegate void AttackEventEventHandler(Enemy Enemy, int id);
    public event AttackEventEventHandler Event; 

    public void SendEventMessage(Enemy Enemy, int id)
    {
        Event?.Invoke(Enemy, id);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<Enemy> EnemyBlackboardVariable = messageData[0] as BlackboardVariable<Enemy>;
        var Enemy = EnemyBlackboardVariable != null ? EnemyBlackboardVariable.Value : default(Enemy);

        BlackboardVariable<int> idBlackboardVariable = messageData[1] as BlackboardVariable<int>;
        var id = idBlackboardVariable != null ? idBlackboardVariable.Value : default(int);

        Event?.Invoke(Enemy, id);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        AttackEventEventHandler del = (Enemy, id) =>
        {
            BlackboardVariable<Enemy> var0 = vars[0] as BlackboardVariable<Enemy>;
            if(var0 != null)
                var0.Value = Enemy;

            BlackboardVariable<int> var1 = vars[1] as BlackboardVariable<int>;
            if(var1 != null)
                var1.Value = id;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as AttackEventEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as AttackEventEventHandler;
    }
}

