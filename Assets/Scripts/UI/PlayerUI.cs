using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] UpgradeIconUI upgradeIconPrefab;
    [SerializeField] GameObject healthIconPrefab;

    [Header("Health")]
    [SerializeField] TMPro.TextMeshProUGUI playerNameText;
    [SerializeField] Transform healthIconHolder;
    [SerializeField] float healthIconFilledAlpha = 0.4f;
    [SerializeField] float healthIconDepletedAlpha = 0.1f;

    [Header("Abilities")]
    [SerializeField] GameObject abilityIcon;

    [Header("Upgrades")]
    [SerializeField] Transform upgradeIconHolder;
    [SerializeField] GameObject upgradeTooltipObj;
    [SerializeField] TMPro.TextMeshProUGUI upgradeTooltipTitleText;
    [SerializeField] TMPro.TextMeshProUGUI upgradeTooltipDescriptionText;

    [Header("Combo")]
    [SerializeField] TMPro.TextMeshProUGUI comboLetterText;
    [SerializeField] Image comboValueBar;

    private Player player;
    private PlayerAbility playerAbility;
    private int currentDisplayHealthMax = 0;

    public void Setup(string playerName, Player player)
    {
        playerNameText.text = playerName;
        this.player = player;
        playerAbility = player.playerAbility;

        ShowHP(player.health.Value);
        ShowAbilityIcon(player.playerAbility.isAbilityAvailable.Value);

        player.health.OnValueChanged += OnPlayerHealthChanged;
        player.playerAbilityRef.OnValueChanged += OnPlayerAbilityChanged;
        player.playerAbility.isAbilityAvailable.OnValueChanged += OnPlayerAbilityIsAvailable;
        player.stats.OnPlayerStatsChangeEvent += OnStatsChanged;
        player.playerCombo.currentCombolevel.OnValueChanged += OnComboLevelChanged;
        ShowComboLevel(player.playerCombo.currentCombolevel.Value);
        Color color = player.color;
        color.a = GetComponent<Image>().color.a;
        GetComponent<Image>().color = color;
        PopulateUpgrades();
    }

    private void OnPlayerAbilityChanged(NetworkBehaviourReference previousValue, NetworkBehaviourReference newValue)
    {
        Debug.Log($"playerAbilChanged {player.OwnerClientId}");
        if(newValue.TryGet(out PlayerAbility newAbility)){
            if(previousValue.TryGet(out PlayerAbility prevAbility)){
                prevAbility.isAbilityAvailable.OnValueChanged -= OnPlayerAbilityIsAvailable;
            }
            newAbility.isAbilityAvailable.OnValueChanged += OnPlayerAbilityIsAvailable;
            playerAbility = newAbility;
        } else
        {
            Debug.LogError("Player UI: Invalid Player ability reference!");
        }

    }

    public void OnDisable()
    {
        player.health.OnValueChanged -= OnPlayerHealthChanged;
        playerAbility.isAbilityAvailable.OnValueChanged -= OnPlayerAbilityIsAvailable;
        player.stats.OnPlayerStatsChangeEvent -= OnStatsChanged;
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
                .Setup(upgrade, upgradeTooltipObj, upgradeTooltipTitleText, upgradeTooltipDescriptionText);
        }
    }

    private void OnStatsChanged()
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
    private void OnComboLevelChanged(int previousValue, int newValue)
    {
        ShowComboLevel(newValue);
    }

    private void UpdateComboBar()
    {
        if (!player.IsOwner) return;
        comboValueBar.fillAmount = player.playerCombo.GetComboLevelFrac();
        
    }
    public void Update()
    {
        UpdateComboBar();
    }

    private void ShowHP(int hp)
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

    private void ShowAbilityIcon(bool isShowing)
    {
        abilityIcon.SetActive(isShowing);
    }

    private void ShowComboLevel(int level)
    {
        string ltr;
        switch (level) //ok to hardcode this prolly
        {
            case 0: 
                ltr = "F";
                break;
            case 1: 
                ltr = "E";
                break;
            case 2: 
                ltr = "D";
                break;
            case 3: 
                ltr = "C";
                break;
            case 4: 
                ltr = "B";
                break;
            case 5: 
                ltr = "A";
                break;
            case 6: 
                ltr = "S";
                break;
            case 7: 
                ltr = "SS";
                break;
            case 8: 
                ltr = "X";
                break;
            case 9: 
                ltr = "XX";
                break;
            case 10: 
                ltr = "XD";
                break;
            default: 
                ltr = "ERR";
                break;
        }
        comboLetterText.text = ltr;

    }
}
