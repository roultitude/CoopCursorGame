using UnityEngine;

public class RotationLock : MonoBehaviour
{
    [SerializeField]
    Vector3 lockToAngles;

    private Quaternion lockTo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lockTo = Quaternion.Euler(lockToAngles);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = lockTo;
    }
}
