using Unity.Netcode;
using UnityEngine;

public class AttackIndicatorManager : NetworkBehaviour
{
    public static AttackIndicatorManager Instance;
    [SerializeField]
    DestroyAfterDelay lineIndicatorPrefab;

    private void Awake()
    {
        if (!Instance) { Instance = this; } else
        {
            Destroy(gameObject);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SpawnLineAttackIndicatorRPC(bool isHorizontal, float width, float locOnAxis, float duration)
    {
        Vector2 pos = new Vector2(isHorizontal ? 0 : locOnAxis, isHorizontal ? locOnAxis : 0);
        Vector2 scale = new Vector2(isHorizontal ? 20 : width, isHorizontal ? width : 20);
        DestroyAfterDelay indicator = Instantiate(lineIndicatorPrefab,pos,Quaternion.identity);
        indicator.Setup(duration);
        indicator.gameObject.transform.localScale = scale;
    }

    [ContextMenu("Trigger Horizontal Indicator")]
    private void DebugTriggerAttackOne()
    {
        SpawnLineAttackIndicatorRPC(true, 5, 0, 2);
    }

    [ContextMenu("Trigger Vertical Indicator")]
    private void DebugTriggerAttackTwo()
    {
        SpawnLineAttackIndicatorRPC(false, 2, 3, 5);
    }
}
