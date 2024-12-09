
using System.Collections.Generic;
using UnityEngine;

public class Boss_HexaHexaGone : Boss
{
    [SerializeField]
    private Enemy hexaPartPrefab;
    
    [SerializeField]
    private Vector2[] partSpawnPos;

    // Variables for Perlin noise-based movement
    private float noiseTime = 0f;
    public float roamRadius;
    public float noiseScale = 1f; // Adjust to control the speed of change in direction
    public float noiseFrequency = 0.5f; // Adjust to control how quickly the direction changes
    private Vector2 startPosition;

    //[SerializeField]
    //private // for particlesystem for spawning effect
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            startPosition = transform.position;
            Invoke(nameof(SpawnParts),1f); 
            // we need the main body to have spawned on all clients else SetupRPC on parts will not register to the parts list
        }
    }

    public override void OnHurt(float num)
    {
        if (parts.Count != 0) return;
        base.OnHurt(num);
    }

    public void SpawnParts()
    {
        for (int i = 0; i < partSpawnPos.Length; i++)
        {
            Enemy part = Instantiate(hexaPartPrefab, partSpawnPos[i] + (Vector2) transform.position,Quaternion.identity);
            part.NetworkObject.Spawn();
            part.SetupRPC(this);
        }
        
    }
    private void Move()
    {
        // Update the noise time
        noiseTime += Time.fixedDeltaTime * noiseFrequency;

        // Generate smooth directional changes using Perlin noise
        float angle = Mathf.PerlinNoise(noiseTime, 0f) * 360f; // 0 to 360 degrees
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // Calculate the potential new position
        Vector2 moveStep = direction * moveSpeed * Time.fixedDeltaTime;
        Vector2 nextPos = (Vector2) transform.position + moveStep;

        // Keep the enemy within the roam radius
        if (Vector2.Distance(startPosition, nextPos) > roamRadius)
        {
            // Redirect towards the center smoothly
            Vector3 toCenter = (startPosition - (Vector2) transform.position).normalized;
            direction = Vector2.Lerp(direction, toCenter, 0.1f).normalized;
            moveStep = (Vector3)direction * moveSpeed * Time.fixedDeltaTime;
            nextPos = (Vector2) transform.position + moveStep;
        }
        transform.position = nextPos;

    }
    protected override void FixedUpdate()
    {
        if (!IsServer) return;
        Move();

        for (int i = 0; i < parts.Count; i++) {
            Enemy part = parts[i];
            part.transform.position = partSpawnPos[i] + (Vector2)transform.position;
        }
    }
}
