namespace Assets.Scripts.Mobs
{
    public class ArmoredDieCondition : DieConditionHandler
    {
        public enum Type
        {
            Even,
            Odd,
        }

        public Type type;

        private int index = 0;

        protected override (int[], string) GenerateCondition()
        {
            if (index >= 3)
                return (new int[] { 1, 2, 3, 4, 5, 6 }, "");

            int condition = 0;
            if (type == Type.Even)
            {
                switch (index)
                {
                    case 0: condition = 2; break;
                    case 1: condition = 4; break;
                    case 2: condition = 6; break;
                }
            }

            if (type == Type.Odd)
            {
                switch (index)
                {
                    case 0: condition = 1; break;
                    case 1: condition = 3; break;
                    case 2: condition = 5; break;
                }
            }

            index++;
            return (new int[] { condition }, condition.ToString());
        }

        public override bool CheckDieCondition(int diceNumber)
        {
            if (base.CheckDieCondition(diceNumber))
            {
                SetCondition();
                return true;
            }

            return false;
        }
    }
}