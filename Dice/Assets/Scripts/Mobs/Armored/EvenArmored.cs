using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class EvenArmored : Armored
    {
        public float moveTime;
        public float breakTime;

        private float moveTimeRemain = 0;
        private float timeFlag = 0;

        protected override void ActionHandle()
        {
            if (moveTimeRemain > 0)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                MoveToDirection(direction);

                moveTimeRemain -= Time.fixedDeltaTime;
                if (moveTimeRemain <= 0)
                {
                    timeFlag = Time.time;
                    SetAnimatorMoving(false);
                }
            }
            else if (Time.time - timeFlag > breakTime)
            {
                StartMoving();
            }

            FaceToPlayer();
        }

        private void StartMoving()
        {
            moveTimeRemain = Random.Range(moveTime * 0.5f, moveTime * 1.5f);
            SetAnimatorMoving(true);
        }

        private void FaceToPlayer()
        {
            Vector3 faceDirection = (player.position - transform.position).normalized;
            FacingToDirection(faceDirection.x);
        }
    }
}