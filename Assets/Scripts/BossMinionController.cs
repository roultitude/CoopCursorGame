using Unity.Netcode;
using UnityEngine;

public class BossMinionController : NetworkBehaviour
{
    private Vector2 targetPosition;
    private int minionId;

    public void Setup(int id)
    {
        minionId = id;
    }

    public void SetTargetPosition(Vector2 pos)
    {
        targetPosition = pos;
    }

    public Vector2 GetTargetPosition()
    {
        return targetPosition;
    }

    public int GetMinionId() {
        return minionId;
    }

}
