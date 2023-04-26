using System;
using UnityEngine;

namespace Assets.Scripts.Dice
{
    public class AnimEventHandler : MonoBehaviour
    {
        public event Action StopRollingAction;

        private void StopRolling() { StopRollingAction?.Invoke(); }
    }
}