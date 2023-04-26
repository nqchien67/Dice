using Assets.Scripts.Dice;
using Assets.Scripts.System.Health;
using Assets.Scripts.System.Wave;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace Assets.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Move")]
        public float moveSpeed;

        public float throwPositionOffset;

        [Header("Recovery")]
        public float recoveryTime;
        private float RemainRecoveryTime;
        public float blinkTime;
        private float blinkTimer;
        public bool IsKnockbacking { get; private set; } = false;

        [Header("Knockback")]
        public float kbStrength;
        public float kbTime;

        [Header("Dash")]
        public float dashForce;
        public float dashLength;
        public float dashCooldown;

        public GameObject dustPrefab;

        private Vector2 dashDirection;
        private float dashCounter;
        private float dashCoolCounter;

        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;
        private Animator animator;
        private AnimEventHandler animEvent;
        private Hurtbox diceHolder;

        private Vector2 movementInput;

        private bool canMove = true;

        [HideInInspector] public bool isHoldingDice;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = transform.GetChild(0).GetComponent<Animator>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animEvent = GetComponentInChildren<AnimEventHandler>();
            diceHolder = GetComponentInChildren<Hurtbox>();
        }

        private void Start()
        {
            blinkTimer = Time.time;
            animEvent.OnEndKnockback += StartRecovery;
            animEvent.OnDead += Dead;

            animator.SetTrigger("spawn");
        }

        private void FixedUpdate()
        {
            MoveHandle();
            DashHandle();
            RecoveryHandle();
        }

        private void MoveHandle()
        {
            if (!canMove)
                return;

            float actualSpeed = isHoldingDice ? moveSpeed * 0.85f : moveSpeed;
            rb.velocity = movementInput * actualSpeed;

            if (movementInput != Vector2.zero)
                animator.SetBool("isMoving", true);
            else
                animator.SetBool("isMoving", false);

            if (movementInput.x < 0)
                spriteRenderer.flipX = true;

            else if (movementInput.x > 0)
                spriteRenderer.flipX = false;
        }

        private void DashHandle()
        {
            if (dashCoolCounter > 0)
                dashCoolCounter -= Time.fixedDeltaTime;

            if (dashCounter > 0)
            {
                rb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
                dashCounter -= Time.fixedDeltaTime;

                if (dashCounter <= 0)
                {
                    dashCoolCounter = dashCooldown;
                }
            }

        }

        private void RecoveryHandle()
        {
            if (RemainRecoveryTime > 0)
            {
                if (Time.time - blinkTimer > blinkTime)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;

                    blinkTimer = Time.time;
                }

                RemainRecoveryTime -= Time.fixedDeltaTime;

                if (RemainRecoveryTime < 0)
                {
                    spriteRenderer.enabled = true;

                }
            }
        }

        public bool Hit(Vector2 knockbackDirection, bool canAvoid = true)
        {
            if (canAvoid &&
                (IsKnockbacking || RemainRecoveryTime > 0 || dashCounter > 0))
                return false;

            HealthBar.instance.LoseHealth();

            animator.SetTrigger("hurt");

            DropDice();

            StartCoroutine(Knockback(knockbackDirection));

            if (HealthBar.instance.GetCurrentHealth() <= 0)
                Die();

            return true;
        }

        private void Die()
        {
            spriteRenderer.sortingLayerName = "GameOver";
            GetComponent<SortingGroup>().enabled = false;
            animator.SetTrigger("die");
        }

        private void DropDice()
        {
            if (isHoldingDice)
            {
                Transform dice = diceHolder.Dice;
                dice.position = transform.position;
                dice.GetComponent<DiceController>().Drop();

                isHoldingDice = false;

                animator.SetBool("isHoldingDice", false);
            }
        }

        private IEnumerator Knockback(Vector2 direction)
        {
            IsKnockbacking = true;
            canMove = false;

            CameraController.instance.StartShake(0.12f, 0.1f);

            rb.velocity = Vector2.zero;
            rb.AddForce(direction * kbStrength, ForceMode2D.Impulse);
            yield return new WaitForSeconds(kbTime);
            canMove = true;
        }

        private void OnMove(InputValue movementValue)
        {
            movementInput = movementValue.Get<Vector2>();
        }

        private void OnFire()
        {
            if (isHoldingDice)
            {
                Vector2 mousePostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (mousePostion - (Vector2)transform.position).normalized;

                Transform dice = diceHolder.Dice;

                RaycastHit2D hitWall = Physics2D.Raycast(transform.position, direction, 1, 1 << 3);
                if (hitWall)
                    dice.position = (Vector2)transform.position - direction * hitWall.distance;
                else
                    dice.position = (Vector2)transform.position + direction * throwPositionOffset;

                dice.GetComponent<DiceController>().Throw(direction);

                isHoldingDice = false;
                animator.SetBool("isHoldingDice", false);
            }
        }

        private void OnDash()
        {
            if (dashCoolCounter <= 0 && dashCounter <= 0 && !IsKnockbacking)
            {
                dashCounter = dashLength;
                dashDirection = Vector2.zero;
                if (movementInput == Vector2.zero)
                    dashDirection.x = spriteRenderer.flipX ? -1 : 1;
                else
                    dashDirection = movementInput;

                animator.SetTrigger("roll");
                RenderDust();
            }
        }

        private void RenderDust()
        {
            GameObject dust = Instantiate(dustPrefab, transform.position, Quaternion.identity);
            dust.GetComponentInChildren<SpriteRenderer>().flipX = !spriteRenderer.flipX;
        }

        #region Animation Trigger
        private void StartRecovery()
        {
            RemainRecoveryTime = recoveryTime;
            IsKnockbacking = false;
        }

        private void Dead()
        {
            rb.simulated = false;
            WaveManager.instance.StopHesAlreadyDead();
            this.enabled = false;
        }
        #endregion
    }
}
