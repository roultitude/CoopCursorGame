using UnityEngine;
[CreateAssetMenu(fileName = "Effect_AddOrbitalOnPickup", menuName = "Scriptable Objects/CustomEffects/Effect_AddOrbitalOnPickup")]
public class Effect_AddOrbitalOnPickup : CustomEffectSO
{
    [SerializeField]
    PlayerOrbital orbitalPrefab;

    [SerializeField]
    bool isSquare;

    public override bool OnAdded(TickableTimer timer, Player player)
    {
        if (!player.IsOwner) return false;
        if (isSquare) player.playerOrbitals.TestSpawnSquareOrb();
        else player.playerOrbitals.TestSpawnTriangleOrb();
        return false;
    }

}
