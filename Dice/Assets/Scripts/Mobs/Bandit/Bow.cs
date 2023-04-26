using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Mobs
{
    public class Bow : MonoBehaviour
    {
        public GameObject arrowPrefab;
        public float shootForce;

        private Animator animator;
        private Transform arrowPosition;

        public bool isAiming;
        private Transform target;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            arrowPosition = transform.Find("ArrowPosition");
        }

        private void Update()
        {
            if (isAiming)
                Aim();
            else
                transform.localRotation = Quaternion.identity;
        }

        private void Aim()
        {
            Vector2 direction = (target.position - transform.position).normalized;

            Flip(direction.x > 0);

            if (direction.x < 0)
                direction = -direction;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = targetRotation;
        }

        public void Flip(bool isFlipToRight)
        {
            if (isFlipToRight)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }

        public void TakeAim(Transform target)
        {
            this.target = target;
            isAiming = true;
            SetAimAnimator();
        }

        public void SetAimAnimator()
        {
            animator.SetTrigger("takeAim");
        }

        public void Shoot()
        {
            isAiming = false;

            Vector2 direction = (target.position - transform.position).normalized;

            GameObject arrowObj = Instantiate(arrowPrefab, arrowPosition.position, CalculateArrowRotation(direction));
            Arrow arrow = arrowObj.GetComponent<Arrow>();

            arrow.shootForce = shootForce;
            arrow.direction = direction;

            animator.SetTrigger("shoot");
        }

        public void Spawn()
        {
            animator.SetTrigger("spawn");
        }

        public void Drop()
        {
            isAiming = false;
            transform.rotation = Quaternion.identity;
            GetComponent<SortingGroup>().sortingLayerName = "DroppedWeapon";
            animator.SetTrigger("drop");
        }

        private Quaternion CalculateArrowRotation(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, 0, -(90 - angle));
        }
    }
}