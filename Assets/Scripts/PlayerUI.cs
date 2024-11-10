using UnityEngine;

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

    private int currentDisplayHealth = 0;

    public void Setup(string playerName, int initialHealth)
    {
        playerNameText.text = playerName;
        ShowHP(initialHealth);
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

    public void ShowAbilityCooldown(bool isShowing)
    {
        abilityIcon.SetActive(isShowing);
    }
}
