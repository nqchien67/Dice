using Assets.Scripts.Dice;
using Assets.Scripts.System.Health;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class BonusHandler : MonoBehaviour
    {
        public GameObject dicePrefab;

        private DiceController[] dices;
        private bool isGettingBonus = false;

        private void Start()
        {
            dices = new DiceController[1];
        }

        private void Update()
        {
            if (!isGettingBonus && AreAllDicesMatch())
            {
                isGettingBonus = true;

                StartCoroutine(RerollAllDices());
                StartCoroutine(GetBonus());
            }
        }

        private bool AreAllDicesMatch()
        {
            GameObject[] diceGOs = GameObject.FindGameObjectsWithTag("Dice");
            if (diceGOs.Length <= 1)
                return false;

            if (diceGOs.Length > dices.Length)
            {
                dices = diceGOs.Select(diceGO => diceGO.GetComponent<DiceController>()).ToArray();
            }

            for (int i = 0; i < dices.Length; i++)
            {
                bool isSameFound = false;
                for (int j = 0; j < dices.Length; j++)
                {
                    if (i != j && AreDiceNumbersEqual(dices[i], dices[j]))
                    {
                        isSameFound = true;
                        break;
                    }
                }

                if (!isSameFound)
                    return false;
            }
            return true;
        }

        private bool AreDiceNumbersEqual(DiceController dice1, DiceController dice2)
        {
            bool areBothDicesNotRolling = dice1.Number != 0 && dice2.Number != 0;
            return (areBothDicesNotRolling && dice1.Number == dice2.Number);
        }

        private IEnumerator RerollAllDices()
        {
            yield return new WaitForSeconds(1);
            foreach (var dice in dices)
                dice.Reroll();
        }

        private IEnumerator GetBonus()
        {
            if (HealthBar.instance.GetCurrentHealth() < 3)
                HealthBar.instance.GainHealth();
            else
            {
                yield return new WaitForSeconds(2);
                Instantiate(dicePrefab, new Vector2(Random.Range(0, 2f), Random.Range(0, 2f)), Quaternion.identity);
            }

            isGettingBonus = false;
            InstructionText.instance.OnIndex6();
        }
    }
}