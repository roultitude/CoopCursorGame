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
    public Sprite sprite;
    public PlayerStatChange[] statChanges;
    public CustomEffectSO customEffect;
}
