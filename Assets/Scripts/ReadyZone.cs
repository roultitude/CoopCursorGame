using Unity.Netcode;
using UnityEngine;

public class ReadyZone : MonoBehaviour
{
    [SerializeField] float countdownTime;
    [SerializeField] Collider2D zoneCollider;
    [SerializeField] ObjectiveUI ui;
    [SerializeField] string sceneName;

    float timer = 0;
    private void FixedUpdate()
    {
        int numReady = 0;
        foreach(Player player in PlayerManager.Instance.players)
        {
            if (player.playerCollider.IsTouching(zoneCollider))
            {
                numReady++;
            }
        }
        if (numReady != PlayerManager.Instance.players.Count)
        {
            timer = 0;
            ui.UpdateObjectiveText($"{numReady} / {PlayerManager.Instance.players.Count} Ready");
            return;
        }
        timer += Time.fixedDeltaTime;
        ui.UpdateObjectiveText(Mathf.Ceil(countdownTime - timer).ToString());

        if(timer >= countdownTime)
        {
            TriggerReadyZoneAction();
            timer = 0;
        }
    }
    
    private void TriggerReadyZoneAction()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.LoadNextGameScene();
    }
}
