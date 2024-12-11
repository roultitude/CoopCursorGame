
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Boss_HexaHexaGone : Boss
{
    [SerializeField]
    private Enemy hexaPartPrefab;

    public Vector2[] partSpawnPos;
    private Vector2 startPosition;
    private BehaviorGraphAgent agent;
    private BlackboardVariable<AttackEvent> attackEventChannel;
    //[SerializeField]
    //private // for particlesystem for spawning effect
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<BehaviorGraphAgent>();
        agent.BlackboardReference.GetVariable<AttackEvent>("AttackEventChannel", out attackEventChannel);
        /*
        if() {
            attackEventChannel = attackEvent;
        } else
        {
            Debug.LogError($"{agent} is expecting a BlackboardVariable of type '{typeof(AttackEvent).Name}' named AttackEventChannel.");
            return;
        }
        */
    }

    private void OnEnable()
    {
        attackEventChannel.Value.Event += AttackEventChannel_OnEvent;
    }
    private void OnDisable()
    {
        attackEventChannel.Value.Event -= AttackEventChannel_OnEvent;
    }

    private void AttackEventChannel_OnEvent(Enemy Enemy, int id)
    {
        Debug.Log("Received AttackChannel Event of id " + id);
        if (id != -1) return; // listen for attack event complete
        TriggerAttackRPC(Random.Range(0, 2));
    }

    public void TriggerRandomAttack()
    {
        TriggerAttackRPC(Random.Range(0, 2));
    }

    [ContextMenu("Trigger Attack 0")]
    private void DebugTriggerAttackZero()
    {
        TriggerAttackRPC(0);
    }
    [ContextMenu("Trigger Attack 1")]
    private void DebugTriggerAttackOne()
    {
        TriggerAttackRPC(1);
    }

    [Rpc(SendTo.Everyone)]
    private void TriggerAttackRPC(int id)
    {
        Debug.Log("Triggered AttackChannel Event of id " + id); 
        attackEventChannel.Value.SendEventMessage(this, id); //doesnt do much lmao
        agent.BlackboardReference.SetVariableValue("AttackId",id); //still need this....
        
            /*
        switch (attackId) {
            case 0:
                attackEventChannel.Value.SendEventMessage(this, attackId);
                break;
            case 1: 
                break;
            default: 
                return;
        }
        */
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            transform.position = startPosition;
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
            part.GetComponent<BehaviorGraphAgent>().BlackboardReference.SetVariableValue<int>("PartNumber", i);
        }
        
    }
    protected override void FixedUpdate()
    {
        if (!IsServer) return;
        //Move();
        /*
        for (int i = 0; i < Mathf.Min(partSpawnPos.Length,partPosition.Count); i++) {
            partPosition[i] = partSpawnPos[i] + (Vector2)transform.position; 
        }*/
    }

    public void RadialBulletAttack()
    {

    }
}
