using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectionTile : MonoBehaviour
{
    public UpgradeSO data;

    [SerializeField]
    BoxCollider2D col;

    [SerializeField]
    SpriteRenderer sprite;

    [SerializeField]
    UpgradeSelection manager;

    [SerializeField]
    List<Collider2D> touchingCol;

    [SerializeField]
    GameObject uiCanvas;

    [SerializeField]
    TMPro.TextMeshProUGUI nameText, descText;
    public void Setup(UpgradeSO upgrade)
    {
        sprite.sprite = upgrade.sprite;
        data = upgrade;
        touchingCol = new List<Collider2D>();
        nameText.text = upgrade.upgradeName;
        descText.text = upgrade.upgradeDesc;
    }

    public List<Player> GetPlayersTouching()
    {
        List<Player> players = new List<Player>();
        bool localPlayerTouching = false;
        foreach(Player player in PlayerManager.Instance.players)
        {
            if (col.IsTouching(player.playerCollider))
            {
                players.Add(player);
                if (player.IsOwner)
                {
                    localPlayerTouching = true;
                }
            }
        }
        uiCanvas.SetActive(localPlayerTouching);
        return players;
    }
}
