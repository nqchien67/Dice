namespace Assets.Scripts.Mobs
{
    public class SlimeAnimEventHandler : MobAnimEventHandler
    {
        private void OnEndJump()
        {
            ((Slime)mob).EndJump();
        }
    }
}