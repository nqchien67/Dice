using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class Warlock : MobController
    {
        public float moveForce;

        protected override void ActionHandle()
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.AddForce(direction * moveForce, ForceMode2D.Impulse);

            float xDirection = (player.position - transform.position).x;
            FacingToDirection(xDirection);
        }
    }
}