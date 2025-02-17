using UnityEngine;

public class BulletSceneInitializer : MonoBehaviour
{
    public static bool hasBeenInitialized = false;

    [SerializeField]
    GameObject BulletSceneInitializerPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!hasBeenInitialized)
        {
            Instantiate(BulletSceneInitializerPrefab);
            hasBeenInitialized = true;
        }
    }

}
