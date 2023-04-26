using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.System.Wave
{
    [Serializable]
    public struct Mob
    {
        public GameObject mob;
        [Range(1, 99)]
        public int amount;
    }

    [Serializable]
    public struct Wave
    {
        public List<Mob> mobs;
    }

    [CreateAssetMenu(menuName = "Scriptable Object/Wave Data")]
    public class WaveData : ScriptableObject
    {
        public Wave[] waves;
    }
}