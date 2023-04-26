using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class BanditDieCondition : DieConditionHandler
    {
        protected override (int[], string) GenerateCondition()
        {
            int number = Random.Range(3, 5);
            bool isLarger = Random.Range(0, 2) > 0;

            if (isLarger)
                return (Enumerable.Range(number + 1, 6 - number).ToArray(), "> " + number);
            else
                return (Enumerable.Range(1, number - 1).ToArray(), "< " + number);
        }
    }
}