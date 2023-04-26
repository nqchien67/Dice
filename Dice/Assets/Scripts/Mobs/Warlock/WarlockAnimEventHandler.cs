using Assets.Scripts.System;

namespace Assets.Scripts.Mobs
{
    public class WarlockAnimEventHandler : MobAnimEventHandler
    {
        protected override void OnEndDieAnimation()
        {
            base.OnEndDieAnimation();
            InstructionText.instance.OnIndex3();
        }
    }
}