using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSO", menuName = "Scriptable Objects/WaveSO")]
public class WaveSO : ScriptableObject
{
    public EnemyGroup[] groups;

    [Serializable]
    public struct EnemyGroup
    {
        public Enemy[] enemies;
    }
}
