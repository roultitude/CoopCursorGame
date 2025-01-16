using Unity.Netcode;
using UnityEngine;

public class PlayerComboManager : NetworkBehaviour
{
    public NetworkVariable<int> currentCombolevel = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float decayRate = 50f;


    [SerializeField]
    private float dmgMultPerLevel = 0.1f;
    [SerializeField]
    private Player player;
    [SerializeField]
    private float timeBeforeDecay = 1f;

    public float GetComboLevelFrac() => (comboValue%100) / 100;
    public float GetComboDmgMult() => 1 + currentCombolevel.Value * dmgMultPerLevel;
    private float comboValue = 0;

    private int maxComboLevel = 6; //1-10, each represents a tier
    private TickableTimer decayTimer;

    //F 0-99
    //E 100-199
    //D 200-299
    //C 300-399
    //B 400-499
    //A 500-599
    //S 600-699
    //SS 700-799
    //X 800-899
    //XX 900-999
    //XD 1000
    private void Awake()
    {
        decayTimer = new CountupTimer(timeBeforeDecay);
    }

    public void SetCombo(float amt)
    {
        comboValue = Mathf.Clamp(amt,0,LvlToMaxVal(maxComboLevel));
        decayTimer.Reset();
        UpdateComboLevel();
    }

    public void ModifyCombo(float amt, bool resetDecayTimer = false)
    {
        if (resetDecayTimer) decayTimer.Reset();
        comboValue = Mathf.Clamp(comboValue + amt, 0, LvlToMaxVal(maxComboLevel));
        UpdateComboLevel();
    }



    private void UpdateComboLevel()
    {
        currentCombolevel.Value = ValToLvl(comboValue);
    }

    private void Update()
    {
        if (!IsOwner) return;
        decayTimer.Tick(Time.deltaTime);
        if (decayTimer.isTimerComplete)
        {
            ModifyCombo(-Time.deltaTime * decayRate);
        }
    }


    public float LvlToMaxVal(int lvl)
    {
        return lvl * 100;
    }

    public int ValToLvl(float val)
    {
        
        return Mathf.FloorToInt(Mathf.Clamp(val,0,1000) / 100);
    }

}
