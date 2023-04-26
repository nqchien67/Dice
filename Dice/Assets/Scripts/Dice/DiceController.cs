using Assets.Scripts.Mobs;
using UnityEngine;

namespace Assets.Scripts.Dice
{
    public class DiceController : MonoBehaviour
    {
        public int nextNumber = 0;

        public float fireForce;
        public float smoothing;
        public float friction;

        public float idleMass;
        public float movingMass;

        public float maxYSpritePos;

        public int Number { get; private set; } = 0;

        private Transform diceHolder;
        private Vector3 currentFollowVelocity = Vector3.zero;

        private Rigidbody2D rb;
        private Collider2D col;
        private Animator animator;
        private Transform sprite;
        private Transform shadow;
        private AnimEventHandler animEventHandler;

        private float minYSpritePos;

        private bool ableToHold = true;
        private bool beingHold = false;
        private bool isReducingVelocity = true;
        private bool ableToRoll = true;

        private float maxShadowScale;
        private float maxDeltaY;

        public bool IsMoving { get; private set; } = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            animator = GetComponentInChildren<Animator>();
            animEventHandler = GetComponentInChildren<AnimEventHandler>();

            sprite = transform.GetChild(0);
            shadow = transform.GetChild(1);
        }

        private void Start()
        {
            diceHolder = GameObject.FindWithTag("Hurtbox").transform;
            minYSpritePos = sprite.localPosition.y;
            maxShadowScale = shadow.localScale.x;

            StartRolling();

            maxDeltaY = maxYSpritePos - minYSpritePos;

            animator.SetLayerWeight(0, 0.5f);
            animator.SetLayerWeight(1, 0.5f);

            animEventHandler.StopRollingAction += StopRolling;
        }

        private void Update()
        {
            if (!col.enabled && !beingHold && sprite.localPosition.y < 0.8)
                col.enabled = true;

            if (IsMoving)
            {
                float speedRatio = rb.velocity.magnitude / 10;
                animator.SetFloat("rollingSpeed", speedRatio);
            }
            else
                animator.SetFloat("rollingSpeed", 1);

            BeingHoldHandle();
            ShadowRender();
        }

        private void ShadowRender()
        {
            float deltaY = sprite.localPosition.y - minYSpritePos;
            float shadowScale = maxShadowScale - deltaY / maxDeltaY * maxShadowScale;

            shadowScale = Mathf.Max(shadowScale, 0.3f);

            shadow.localScale = new Vector3(shadowScale, shadowScale, shadow.localScale.z);
        }

        private void FixedUpdate()
        {
            FrictionSimulation();
        }

        private void BeingHoldHandle()
        {
            if (beingHold)
            {
                transform.position = Vector3.SmoothDamp(transform.position, diceHolder.position, ref currentFollowVelocity, smoothing);
            }
        }

        private void FrictionSimulation()
        {
            if (isReducingVelocity)
            {
                rb.velocity = new Vector2(
                    Mathf.Lerp(rb.velocity.x, 0, friction),
                    Mathf.Lerp(rb.velocity.y, 0, friction));

                if (rb.velocity.magnitude < 5f &&
                    IsMoving &&
                    sprite.localPosition.y <= minYSpritePos)
                {
                    StopRolling();
                }

                if (rb.velocity.magnitude < 0.1f)
                {
                    rb.velocity = Vector2.zero;
                    rb.mass = idleMass;
                    IsMoving = false;
                }
            }
        }

        public void Throw(Vector2 direction)
        {
            rb.mass = movingMass;
            OnUnPickUp();

            rb.AddForce(direction * fireForce, ForceMode2D.Impulse);

            ableToHold = false;
            ableToRoll = true;
            isReducingVelocity = false;

            IsMoving = true;
        }

        public void Drop()
        {
            OnUnPickUp();
            ableToHold = true;
        }

        private void OnUnPickUp()
        {
            col.enabled = true;
            beingHold = false;
            shadow.gameObject.SetActive(true);
            sprite.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        }

        public bool TryPickUp()
        {
            if (ableToHold && Number != 0)
            {
                col.enabled = false;
                beingHold = true;
                IsMoving = false;

                shadow.gameObject.SetActive(false);
                sprite.GetComponent<SpriteRenderer>().sortingLayerName = "Dice";

                return true;
            }

            return false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Collider2D otherCollider = collision.collider;
            if (otherCollider.CompareTag("Walls") ||
                otherCollider.CompareTag("Mob") ||
                otherCollider.CompareTag("Dice"))
            {
                isReducingVelocity = true;
                ableToHold = true;

                if (IsMoving)
                {
                    if (otherCollider.TryGetComponent<MobController>(out var mob) && ableToRoll)
                        mob.TakeHit(Number);

                    if (ableToRoll)
                    {
                        ableToRoll = false;
                        StartRolling();
                    }

                    foreach (Arrow ar in GetComponentsInChildren<Arrow>())
                        Destroy(ar.gameObject);
                }
            }
        }

        public void StartRolling()
        {
            Number = 0;
            animator.SetBool("isRolling", true);
        }

        private void StopRolling()
        {
            if (Number == 0)
            {
                if (nextNumber != 0)
                    Number = nextNumber;
                else
                    Number = Random.Range(1, 7);

                animator.SetInteger("number", Number);
                animator.SetBool("isRolling", false);
            }
        }

        public void Reroll()
        {
            if (!beingHold)
            {
                StartRolling();
                animator.SetTrigger("bounce");
            }
        }

        //private IEnumerator Fall(float startFallHeight, float duration)
        //{
        //    float time = 0f;
        //    shadow.localScale = new Vector3(0.7f, 0.7f, 1);

        //    while (time < duration)
        //    {
        //        time += Time.deltaTime;
        //        float ratio = Mathf.Clamp01(time / duration);
        //        float yPosition = Mathf.Lerp(startFallHeight, originSpritePos.y, ratio);
        //        sprite.localPosition = new Vector3(sprite.localPosition.x, yPosition, sprite.localPosition.z);

        //        if (yPosition < 0.8)
        //            col.enabled = true;

        //        yield return new WaitForEndOfFrame();
        //    }
        //    sprite.localPosition = new Vector3(sprite.localPosition.x, originSpritePos.y, sprite.localPosition.z);
        //    shadow.localScale = new Vector3(0.9f, 0.9f, 1);
        //    StopRolling();
        //}
    }
}