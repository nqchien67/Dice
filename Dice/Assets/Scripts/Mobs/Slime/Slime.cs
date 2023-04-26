using Assets.Scripts.Dice;
using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class Slime : MobController
    {
        public float jumpForce;
        public float jumpTime;
        public float pushFriction;

        private float timer;
        private bool canFlip = true;

        protected override void Start()
        {
            base.Start();
            jumpTime = Random.Range(jumpTime - 0.2f, jumpTime + 0.2f);
            timer = Time.time;
        }

        protected override void ActionHandle()
        {
            if (Time.time - timer > jumpTime)
            {
                Vector2 direction = player.position - transform.position;

                Jump(direction.normalized);
                animator.SetTrigger("jump");
            }

            if (canFlip)
                FacingToDirection((player.position - transform.position).x);
        }

        private void Jump(Vector2 direction)
        {
            currentFriction = 0;
            rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
            canFlip = false;
            timer = Time.time;
        }

        public void EndJump()
        {
            currentFriction = normalFriction;
            canFlip = true;
        }

        protected override void OnCollisionWithDice(Collision2D dice)
        {
            if (dice.transform.GetComponent<DiceController>().IsMoving)
                currentFriction = pushFriction;
        }
    }
}