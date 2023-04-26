using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class OddArmored : Armored
    {
        public float waitTime;
        public float maxMoveDistance;

        private float waitTimeRemain = 0.1f;
        private Vector2 positionToMove;

        protected override void ActionHandle()
        {
            if (waitTimeRemain > 0)
            {
                waitTimeRemain -= Time.fixedDeltaTime;

                if (waitTimeRemain <= 0)
                    StartMoving();
            }
            else
            {
                if (Vector2.Distance(positionToMove, transform.position) > 0.01f)
                {
                    MoveToPosition();
                }
                else
                {
                    SetAnimatorMoving(false);
                    waitTimeRemain = Random.Range(waitTime * 0.5f, waitTime * 1.5f);
                }
            }
        }

        private void StartMoving()
        {
            positionToMove = GetRandomPosition();
            FaceToPosition();
            SetAnimatorMoving(true);
        }

        private void MoveToPosition()
        {
            Vector2 direction = positionToMove - (Vector2)transform.position;
            MoveToDirection(direction.normalized);
        }

        private Vector2 GetRandomPosition()
        {
            Vector2 minPosition = new Vector2(-7, -4);
            Vector2 maxPosition = new Vector2(7, 5);

            Vector2 randomPos = new Vector2(Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y));

            return Vector2.ClampMagnitude(randomPos, maxMoveDistance);
        }

        private void FaceToPosition()
        {
            float xDirection = (positionToMove - (Vector2)transform.position).normalized.x;
            FacingToDirection(xDirection);
        }

        protected override void OnCollisionWithDice(Collision2D dice)
        {
            positionToMove = GetRandomPosition();
            FaceToPosition();
        }
    }
}