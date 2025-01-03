using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    protected List<Enemy> parts;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        parts = new List<Enemy>();
        BossUI.Instance.Setup(this);
    }

    public void TrackPart(bool isTracking, Enemy part)
    {
        if (isTracking && !parts.Contains(part))
        {
            parts.Add(part);
        }
        else if (parts.Contains(part)) { 
            parts.Remove(part);
        }
    }
    // can insert UI hooks for boss UI here.
}
