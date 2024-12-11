using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    protected List<Enemy> parts;

    public List<Vector2> partPosition; //indexed via UID assigned upon part spawn

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        parts = new List<Enemy>();
        partPosition = new List<Vector2>();
    }

    public void TrackPart(bool isTracking, Enemy part)
    {
        if (isTracking && !parts.Contains(part))
        {
            parts.Add(part);
            partPosition.Add(new Vector2(0,0));
        }
        else if (parts.Contains(part)) { 
            parts.Remove(part);
        }
    }
    // can insert UI hooks for boss UI here.
}
