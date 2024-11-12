using System;
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI playerNameText;

    [SerializeField]
    GameObject healthIconPrefab;

    [SerializeField]
    Transform healthIconHolder;

    [SerializeField]
    GameObject abilityIcon;

    private Player player;
    private PlayerAbilitySwipe playerAbility;
    private int currentDisplayHealth = 0;

    public void Setup(string playerName, Player player)
    {
        playerNameText.text = playerName;
        this.player = player;
        ShowHP(player.health.Value);
        ShowAbilityIcon(player.playerAbility.isAbilityAvailable.Value);
        player.health.OnValueChanged += OnPlayerHealthChanged;
        playerAbility = player.playerAbility;
        player.playerAbility.isAbilityAvailable.OnValueChanged += OnPlayerAbilityIsAvailable;

        UnityEngine.Random.InitState((int) player.OwnerClientId);
        
        UnityEngine.Color color = UnityEngine.Random.ColorHSV();
        color.a = GetComponent<Image>().color.a;
        GetComponent<Image>().color = color;
    }

    private void OnPlayerAbilityIsAvailable(bool previousValue, bool newValue)
    {
        ShowAbilityIcon(newValue);
    }

    private void OnPlayerHealthChanged(int previousValue, int newValue)
    {
        ShowHP(newValue);
    }

    public void OnDisable()
    {
        player.health.OnValueChanged -= OnPlayerHealthChanged;
        playerAbility.isAbilityAvailable.OnValueChanged -= OnPlayerAbilityIsAvailable;
    }

    public void ShowHP(int hp)
    {
        Debug.Log($"ShowHP: {currentDisplayHealth},{hp}");
        while (currentDisplayHealth < hp)
        {
            Instantiate(healthIconPrefab, healthIconHolder);
            currentDisplayHealth++;
        }
        while(currentDisplayHealth > hp)
        {
            Transform child = healthIconHolder.transform.GetChild(healthIconHolder.transform.childCount - 1);
            child.SetParent(null);
            Destroy(child.gameObject);
            
            currentDisplayHealth--;
        }
    }

    public void ShowAbilityIcon(bool isShowing)
    {
        abilityIcon.SetActive(isShowing);
    }
}
