using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class Golem : MobController
    {
        public float moveForce;

        public float rollForce;
        public float rollCooldown;
        public float rollFriction;
        public float slowestRollSpeed;
        public float distanceToIgnoreCollision;

        public float chargeTime;
        private float chargeTimeRemain;

        public LayerMask diceLayer;

        private float rollCoolRemain;
        private bool isRolling;
        private float maxRollingSpeed;

        private Vector2 oldVelocity;
        private bool ignoreCollision;

        protected override void Start()
        {
            base.Start();
            rollCoolRemain = rollCooldown;
            dieCondition.SetCanNotDie();
        }

        protected override void ActionHandle()
        {
            RollHandle();
            MoveToPlayer();

            IgnoreCollisionWithDice();
        }

        private void MoveToPlayer()
        {
            if (!isRolling && rollCoolRemain > 0 && chargeTimeRemain <= 0)
            {
                Vector3 direction = (player.position - transform.position).normalized;

                FacingToDirection(direction.x);
                rb.AddForce(direction * moveForce, ForceMode2D.Impulse);
            }
        }

        private void RollHandle()
        {
            if (rollCoolRemain > 0)
            {
                rollCoolRemain -= Time.fixedDeltaTime;
                if (rollCoolRemain < 0)
                    StartCharge();
            }

            if (chargeTimeRemain > 0)
            {
                chargeTimeRemain -= Time.fixedDeltaTime;
                rb.velocity = Vector2.zero;
                if (chargeTimeRemain < 0)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    StartRoll(direction);
                }
            }

            if (isRolling)
            {
                float speedRatio = rb.velocity.magnitude / maxRollingSpeed;
                animator.SetFloat("rollingSpeed", speedRatio);

                FacingToDirection(rb.velocity.x);

                if (IsRollNearlyEnd() && !ignoreCollision)
                    EndRoll();
            }
        }

        private void StartCharge()
        {
            animator.SetTrigger("charge");
            isFrictionEnable = true;
            chargeTimeRemain = chargeTime;
        }

        private void StartRoll(Vector2 direction)
        {
            currentFriction = rollFriction;

            rb.AddForce(direction * rollForce, ForceMode2D.Impulse);

            maxRollingSpeed = rb.velocity.magnitude;
            isRolling = true;

            FacingToDirection(direction.x);

            animator.SetBool("isRoll", true);
        }

        private void EndRoll()
        {
            isRolling = false;
            currentFriction = (normalFriction + rollFriction) / 2f;
            animator.SetBool("isRoll", false);
            dieCondition.SetCanDie();
        }

        public void OnEndStandUp()
        {
            rollCoolRemain = rollCooldown;
            currentFriction = normalFriction;
            dieCondition.SetCanNotDie();
        }

        private bool IsRollNearlyEnd()
        {
            return rb.velocity.magnitude < slowestRollSpeed;
        }

        protected override void OnCollisionWithDice(Collision2D dice)
        {
            if (isRolling)
            {
                rb.velocity = oldVelocity;
                ignoreCollision = false;
            }
        }

        protected override void OnCollisionWithOther(Collision2D other)
        {
            if (other.collider.CompareTag("Walls") && isRolling)
            {
                float rollingForce = rb.velocity.magnitude;
                CameraController.instance.StartShake(rollingForce * 0.006f, rollingForce * 0.05f);
            }
        }

        private void IgnoreCollisionWithDice()
        {
            if (isRolling &&
                !IsRollNearlyEnd() &&
                Physics2D.OverlapCircle(col.bounds.center, distanceToIgnoreCollision, diceLayer))
            {
                oldVelocity = rb.velocity;
                ignoreCollision = true;
            }
            else ignoreCollision = false;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(col.bounds.center, distanceToIgnoreCollision);
        }
    }
}