using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public static BossUI Instance; // TODO: could refactor into some UIServiceLocator later

    [SerializeField]
    Image hpBarImage;
    [SerializeField]
    TextMeshProUGUI bossNameText;

    Boss boss;

    private void Awake()
    {
        if(!Instance) Instance = this;
        else { Destroy(gameObject); return; }
        gameObject.SetActive(false);
    }

    public void Setup(Boss boss)
    {
        this.boss = boss;
        bossNameText.text = boss.gameObject.name;
        gameObject.SetActive(true);
    }
    public void UpdateGraphic()
    {
        hpBarImage.fillAmount = boss.GetHealthFraction();
    }
    public void Update()
    {
        if (boss)
        {
            UpdateGraphic();
        }
    }
}
