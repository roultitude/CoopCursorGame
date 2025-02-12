using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyDashTowards", story: "[Enemy] Dashes towards [Direction]", category: "Action/Enemy", id: "934b9f6f3e0d22b24f62256ed5be2af1")]
public partial class EnemyDashTowardsAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Vector2> Direction;
    [SerializeReference] public BlackboardVariable<bool> IsPosition; //treat direction as position
    [SerializeReference] public BlackboardVariable<float> DashSpeed;
    [SerializeReference] public BlackboardVariable<float> DashDuration;
    [SerializeReference] public BlackboardVariable<bool> SnapRotateTowardsDir; //treat direction as position

    private Vector2 dir;
    private Vector2 bottomLeft;
    private Vector2 topRight;
    private float timer;
    protected override Status OnStart()
    {
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        timer = DashDuration.Value;
        if (IsPosition)
        {
            dir = (Direction.Value - (Vector2) Enemy.Value.transform.position).normalized;
        } else
        {
            dir = Direction.Value.normalized;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Vector2 pos = Enemy.Value.transform.position;
        pos += dir * DashSpeed * Time.deltaTime;
        // Left boundary
        if (pos.x < bottomLeft.x)
        {
            pos.x = bottomLeft.x;          // Clamp position
            dir.x = -dir.x;      // Reverse X velocity
        }
        // Right boundary
        else if (pos.x > topRight.x)
        {
            pos.x = topRight.x;
            dir.x = -dir.x;
        }

        // Bottom boundary
        if (pos.y < bottomLeft.y)
        {
            pos.y = bottomLeft.y;
            dir.y = -dir.y;      // Reverse Y velocity
        }
        // Top boundary
        else if (pos.y > topRight.y)
        {
            pos.y = topRight.y;
            dir.y = -dir.y;
        }

        Enemy.Value.transform.position = pos;
        Enemy.Value.SnapRotation(dir, 90);
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            return Status.Success;
        }
        return Status.Running;
        
    }

    protected override void OnEnd()
    {
    }
}

