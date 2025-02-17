using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsPlayerAlive", story: "[Player] is Alive", category: "Conditions", id: "67e28f88df17ff47eab6abb5858ff4fa")]
public partial class IsPlayerAliveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Player> Player;

    public override bool IsTrue()
    {
        if(Player.Value.isDead.Value) return false;
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
