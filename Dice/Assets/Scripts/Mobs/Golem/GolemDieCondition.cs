using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class GolemDieCondition : DieConditionHandler
    {
        protected override (int[], string) GenerateCondition()
        {
            int number = Random.Range(1, 7);
            return (new int[] { number }, number.ToString());
        }
    }
}
