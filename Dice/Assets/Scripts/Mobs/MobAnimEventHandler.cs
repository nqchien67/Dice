using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class MobAnimEventHandler : MonoBehaviour
    {
        protected MobController mob;

        private void Awake()
        {
            mob = GetComponentInParent<MobController>();
        }

        private void OnEndSpawn()
        {
            mob.OnEndSpawn();
        }

        protected virtual void OnEndDieAnimation()
        {
            StartCoroutine(mob.Disappear(0.15f));
        }

        private void HitGround()
        {
            mob.OnHitGround();
        }
    }
}