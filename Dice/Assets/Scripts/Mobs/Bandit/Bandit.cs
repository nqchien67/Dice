using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class Bandit : MobController
    {
        public float shootRange;
        public float shootCooldown;
        public float aimTime;

        public float moveForce;

        private float shootCoolRemaining;
        private float aimTimeRemaining;

        private Bow bow;

        protected override void Start()
        {
            base.Start();
            bow = GetComponentInChildren<Bow>();
            shootCoolRemaining = shootCooldown;
        }

        protected override void ActionHandle()
        {
            AimingHandler();
            ShootHandler();
            MoveToPlayer();

            float xDirection = (player.position - transform.position).x;
            bool isFacingRight = FacingToDirection(xDirection);
            bow.Flip(isFacingRight);
        }

        private void AimingHandler()
        {
            if (bow.isAiming) return;

            if (shootCoolRemaining > 0)
            {
                shootCoolRemaining -= Time.fixedDeltaTime;
            }

            if (shootCoolRemaining <= 0 &&
                Vector2.Distance(transform.position, player.position) < shootRange)
            {
                aimTimeRemaining = aimTime;
                bow.TakeAim(player);
                animator.SetBool("isAiming", true);
            }
        }

        private void ShootHandler()
        {
            if (aimTimeRemaining > 0)
            {
                aimTimeRemaining -= Time.fixedDeltaTime;

                if (aimTimeRemaining <= 0)
                {
                    bow.Shoot();
                    animator.SetBool("isAiming", false);
                    shootCoolRemaining = Random.Range(shootCooldown - 0.5f, shootCooldown + 0.5f);
                }
            }
        }

        private void MoveToPlayer()
        {
            if (aimTimeRemaining <= 0)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                rb.AddForce(direction * moveForce, ForceMode2D.Impulse);
            }
        }

        protected override void OnCollisionWithDice(Collision2D dice)
        {
            if (aimTimeRemaining > 0)
            {
                aimTimeRemaining = aimTime;
                bow.SetAimAnimator();
            }
        }

        public override void OnEndSpawn()
        {
            base.OnEndSpawn();
            bow.Spawn();
        }

        public override void Die()
        {
            base.Die();
            bow.Drop();
        }
    }
}