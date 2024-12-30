using Unity.Netcode;
using UnityEngine;

public class BossMinionController : NetworkBehaviour
{
    public bool isControlledByBoss;
    private Vector2 targetPosition;
    private int minionId;
    [SerializeField]
    private Enemy enemy;
    [SerializeField]
    private EnemyContactDamageHandler enemyContactDamageHandler;

    public void Setup(int id)
    {
        minionId = id;
    }

    public void SetBossControl(bool isControlledByBoss)
    {
        this.isControlledByBoss = isControlledByBoss;
    }

    public void SetEnemyContactDamageState(bool isDamaging)
    {
        SetEnemyContactDamageStateRPC(isDamaging);
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

    [Rpc(SendTo.Everyone)]
    public void PlayBulletEmitterRPC(int idx)
    {
        if (enemy.bulletEmitters.Length > idx) enemy.bulletEmitters[idx].Boot();
    }

    [Rpc(SendTo.Everyone)]
    public void SetEnemyContactDamageStateRPC(bool isDamaging)
    {
        enemyContactDamageHandler.SetContactDamageState(isDamaging);
    }
}
