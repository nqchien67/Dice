namespace Assets.Scripts.Mobs
{
    public class GolemAnimEventHandler : MobAnimEventHandler
    {
        private void OnEndStandUp()
        {
            ((Golem)mob).OnEndStandUp();
        }
    }
}
