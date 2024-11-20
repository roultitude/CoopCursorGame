using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Image iconImage;
    private GameObject upgradeTooltip;   
    private TMPro.TextMeshProUGUI upgradeTitleText, upgradeDescText;
    private UpgradeSO data;
    public void Setup(UpgradeSO data, GameObject tooltipObj, TMPro.TextMeshProUGUI titleText, TMPro.TextMeshProUGUI descText)
    {
        this.data = data;
        upgradeTooltip = tooltipObj;
        upgradeTitleText = titleText;
        upgradeDescText = descText;
        iconImage.sprite = data.sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        upgradeTooltip.SetActive(true);
        upgradeTitleText.text = data.upgradeName;
        upgradeDescText.text = data.upgradeDesc;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        upgradeTooltip?.SetActive(false);
    }

}
