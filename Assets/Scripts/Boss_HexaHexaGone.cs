
using BulletPro;
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

    public Vector2[] partPosition;
    private List<BossMinionController> minionControllers;
    //[SerializeField]
    //private // for particlesystem for spawning effect
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<BehaviorGraphAgent>();
        minionControllers = new List<BossMinionController>();
        partPosition = new Vector2[partSpawnPos.Length];
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
        AttackEvent attackEvent = ScriptableObject.CreateInstance<AttackEvent>();
        agent.BlackboardReference.SetVariableValue<AttackEvent>("AttackEventChannel", attackEvent);
        agent.BlackboardReference.GetVariable<AttackEvent>("AttackEventChannel", out attackEventChannel);
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
        int next = Random.Range(0, 2);
        Debug.Log($"broadcasting: {next}");
        //TriggerAttackRPC(next);
    }
    /*
    public void TriggerRandomAttack()
    {
        TriggerAttackRPC(Random.Range(0, 2));
    }*/

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
        //Debug.Log("Triggered AttackChannel Event of id " + id); 
        attackEventChannel.Value.SendEventMessage(this, id); 
        
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
            part.NetworkObject.Spawn(true);
            part.SetupRPC(this);
            BossMinionController minionController = part.GetComponent<BossMinionController>();
            minionController.Setup(i);
            minionControllers.Add(minionController);
            minionController.SetBossControl(true);
        }
        
    }
    protected override void FixedUpdate()
    {
        if (!IsServer) return;
        foreach (BossMinionController minionController in minionControllers)
        {
            if(minionController != null) minionController.SetTargetPosition(partPosition[minionController.GetMinionId()]);
        }
        //Move();
        /*
        for (int i = 0; i < Mathf.Min(partSpawnPos.Length,partPosition.Count); i++) {
            partPosition[i] = partSpawnPos[i] + (Vector2)transform.position; 
        }*/
    }

    public void TentacleAttackChase(bool isActive)
    {
        foreach (BossMinionController minionController in minionControllers)
        {
            if (minionController == null) continue;
            minionController.SetBossControl(!isActive);
            minionController.SetEnemyContactDamageState(isActive);
        }
    }

    public void RadialBulletAttack()
    {
        foreach(BossMinionController minionController in minionControllers)
        {
            if (minionController != null) minionController.PlayBulletEmitterRPC(0); 
        }
    }

    public void SpinAttack(float attackYCoord, bool isFromLeft, float delay)
    {
        AttackIndicatorManager.Instance.SpawnLineAttackIndicatorRPC(true, 5, attackYCoord, delay);
    }
}
