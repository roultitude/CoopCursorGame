using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Image iconImage;
    private GameObject upgradeTooltip;   
    private TMPro.TextMeshProUGUI upgradeTitleText, upgradeDescText;
    private Upgrade upg;
    public void Setup(Upgrade upg, GameObject tooltipObj, TMPro.TextMeshProUGUI titleText, TMPro.TextMeshProUGUI descText)
    {
        this.upg = upg;
        upgradeTooltip = tooltipObj;
        upgradeTitleText = titleText;
        upgradeDescText = descText;
        iconImage.sprite = upg.GetData().sprite;
    }
    public void FixedUpdate()
    {
        //if (upg != null) Debug.Log($"{upg.GetData().name}: {upg.GetTimerFraction()}");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        upgradeTooltip.SetActive(true);
        upgradeTitleText.text = upg.GetData().upgradeName;
        upgradeDescText.text = upg.GetData().upgradeDesc;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        upgradeTooltip?.SetActive(false);
    }

}
