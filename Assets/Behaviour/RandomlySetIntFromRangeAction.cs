using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Randomly set Int from Range", story: "[Int] is set to a random Integer from Range (INTEGER ONLY)", category: "Action", id: "3618deb815d4eae86994e0ee188b85cb")]
public partial class RandomlySetIntFromRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Int;
    [SerializeReference] public BlackboardVariable<Vector2> Range;
    [SerializeReference] public BlackboardVariable<int> PrevInt;
    [SerializeReference] public BlackboardVariable<bool> AllowRepeat;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        int nextAtk = UnityEngine.Random.Range((int)Range.Value.x, (int)Range.Value.y + 1);
        if (!AllowRepeat)
        {
            while (nextAtk == PrevInt) { nextAtk = UnityEngine.Random.Range((int)Range.Value.x, (int)Range.Value.y + 1); }
        }
        
        Int.Value = nextAtk;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

