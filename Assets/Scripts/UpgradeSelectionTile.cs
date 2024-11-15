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
    public void Setup(UpgradeSO upgrade)
    {
        sprite.sprite = upgrade.sprite;
        data = upgrade;
        touchingCol = new List<Collider2D>();
    }

    public List<Player> GetPlayersTouching()
    {
        List<Player> players = new List<Player>();
        foreach(Player player in PlayerManager.Instance.players)
        {
            if (col.IsTouching(player.playerCollider))
            {
                players.Add(player);
            }
        }
        return players;
    }
}
