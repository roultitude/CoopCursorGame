using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    [Serializable]
    public struct PlayerStatChange
    {
        public PlayerStatType type;
        public float amt;
    }
    public string upgradeName;
    [TextArea(1,10)]
    public string upgradeDesc;
    public Sprite sprite;
    public PlayerStatChange[] statChanges;
    public CustomEffectSO customEffect;
}
