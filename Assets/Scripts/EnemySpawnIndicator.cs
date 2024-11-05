using UnityEngine;

public class EnemySpawnIndicator : MonoBehaviour
{
    //maybe dont need to network this
    //private float spawnDuration;

    public void Setup(float spawnDuration)
    {
        Invoke(nameof(Destroy), spawnDuration);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

}
