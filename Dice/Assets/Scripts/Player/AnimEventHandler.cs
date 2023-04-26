using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class AnimEventHandler : MonoBehaviour
    {
        public event Action OnEndKnockback;
        public Action OnDead;

        private void EndKnockback() { OnEndKnockback?.Invoke(); }
        private void Dead() { OnDead?.Invoke(); }
    }
}