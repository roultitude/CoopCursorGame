using UnityEngine;
using UnityEngine.U2D;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField]
    Transform playerRoot;
    [SerializeField]
    float maxRotAngle = 10f;
    [SerializeField]
    float rotSpeed;
    Vector2 lastPos;
    Quaternion targetRot;

    private void Awake()
    {
        lastPos = playerRoot.position;
    }
    void Update()
    {
        if (lastPos == (Vector2) playerRoot.position) return;
        Vector2 dir = lastPos - (Vector2) playerRoot.position;
        SetTargetRot(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
        lastPos = playerRoot.position;
    }

    public void SetTargetRot(Vector2 faceDir) // MOVE
    {
        float deg = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg;
        if (deg >= maxRotAngle && deg <= 90) deg = maxRotAngle;
        else if (deg >= 90 && deg <= 180 - maxRotAngle) deg = 180 - maxRotAngle;
        else if (deg >= maxRotAngle - 180 && deg <= -90) deg = maxRotAngle - 180;
        else if (deg <= -maxRotAngle && deg >= -90) deg = -maxRotAngle;
        targetRot = Quaternion.AngleAxis(deg + 90, Vector3.forward);

    }
}
