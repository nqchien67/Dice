using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class SlimeDieCondition : DieConditionHandler
    {
        protected override (int[], string) GenerateCondition()
        {
            int number = Random.Range(0, 2);
            if (number == 0)
                return (new int[] { 2, 4, 6 }, "Even");
            else
                return (new int[] { 1, 3, 5 }, "Odd");
        }
    }
}