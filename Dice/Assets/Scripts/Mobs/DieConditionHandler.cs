using TMPro;
using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public abstract class DieConditionHandler : MonoBehaviour
    {
        protected int[] numbers;

        private TextMeshPro text;
        private bool canDie = true;

        private void Awake()
        {
            text = GetComponentInChildren<TextMeshPro>();
        }

        private void Start()
        {
            SetCondition();
        }

        protected void SetCondition()
        {
            (numbers, text.text) = GenerateCondition();
        }

        public virtual bool CheckDieCondition(int diceNumber)
        {
            if (!canDie) return false;

            foreach (int i in numbers)
            {
                if (diceNumber == i)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetCanNotDie()
        {
            canDie = false;
            text.enabled = false;
        }

        public void SetCanDie()
        {
            canDie = true;
            text.enabled = true;
        }

        protected abstract (int[], string) GenerateCondition();
    }
}