using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI playerNameText;
    [SerializeField] UpgradeIconUI upgradeIconPrefab;
    [SerializeField] GameObject healthIconPrefab;

    [SerializeField] Transform healthIconHolder;
    [SerializeField] Transform upgradeIconHolder;
    [SerializeField] GameObject upgradeTooltipObj;
    [SerializeField] TMPro.TextMeshProUGUI upgradeTooltipTitleText;
    [SerializeField] TMPro.TextMeshProUGUI upgradeTooltipDescriptionText;

    [SerializeField] GameObject abilityIcon;

    [SerializeField] float healthIconFilledAlpha = 0.4f;
    [SerializeField] float healthIconDepletedAlpha = 0.1f;

    private Player player;
    private PlayerAbilitySwipe playerAbility;
    private int currentDisplayHealthMax = 0;

    public void Setup(string playerName, Player player)
    {
        playerNameText.text = playerName;
        this.player = player;
        playerAbility = player.playerAbility;

        ShowHP(player.health.Value);
        ShowAbilityIcon(player.playerAbility.isAbilityAvailable.Value);

        player.health.OnValueChanged += OnPlayerHealthChanged;
        player.playerAbility.isAbilityAvailable.OnValueChanged += OnPlayerAbilityIsAvailable;
        player.stats.OnPlayerStatsChangeEvent += OnStatsChange;
        Color color = player.color;
        color.a = GetComponent<Image>().color.a;
        GetComponent<Image>().color = color;
        PopulateUpgrades();
    }

    private void PopulateUpgrades()
    {
        foreach(Transform child in upgradeIconHolder)
        {
            Destroy(child.gameObject); // pooling prolly not req here
        }
        foreach(Upgrade upgrade in player.upgrades.GetUpgrades())
        {
            Instantiate(upgradeIconPrefab, upgradeIconHolder)
                .Setup(upgrade.GetData(), upgradeTooltipObj, upgradeTooltipTitleText, upgradeTooltipDescriptionText);
        }
    }

    private void OnStatsChange()
    {
        ShowHP(player.health.Value); //force refresh
        PopulateUpgrades();
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
        player.stats.OnPlayerStatsChangeEvent -= OnStatsChange;
    }

    public void ShowHP(int hp)
    {
        //adjust max hp display
        while (currentDisplayHealthMax < player.stats.GetStat(PlayerStatType.MaxHealth))
        {
            Instantiate(healthIconPrefab, healthIconHolder);
            currentDisplayHealthMax++;
        }
        while(currentDisplayHealthMax > player.stats.GetStat(PlayerStatType.MaxHealth))
        {
            Transform child = healthIconHolder.transform.GetChild(healthIconHolder.transform.childCount - 1);
            child.SetParent(null);
            Destroy(child.gameObject);
            
            currentDisplayHealthMax--;
        }
        for(int i = 0; i< healthIconHolder.transform.childCount; i++)
        {
            Image icon = healthIconHolder.transform.GetChild(i).GetComponent<Image>();
            Color color = player.color;
            color.a = i < hp ? healthIconFilledAlpha : healthIconDepletedAlpha;
            icon.color = color;
        }
    }

    public void ShowAbilityIcon(bool isShowing)
    {
        abilityIcon.SetActive(isShowing);
    }
}
