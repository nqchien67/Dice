using Assets.Scripts;
using Assets.Scripts.Mobs;
using System.Collections;
using UnityEngine;

public abstract class MobController : MonoBehaviour
{
    public bool disabledDie = false;

    public float normalFriction;
    public float maxYSpritePosition;

    protected float currentFriction;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform player;
    protected DieConditionHandler dieCondition;
    protected Collider2D col;

    protected Transform sprite;
    private Transform shadow;

    private float maxShadowScale;
    protected float minYSpritePosition;
    private float maxDeltaY;

    private bool canMove = false;
    protected bool isFrictionEnable = true;
    public bool CanAttack { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        col = GetComponent<Collider2D>();
        col.enabled = false;

        dieCondition = GetComponent<DieConditionHandler>();

        sprite = transform.GetChild(0);

        shadow = transform.GetChild(1);
        minYSpritePosition = sprite.localPosition.y;
        maxShadowScale = shadow.localScale.x;
    }

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        maxDeltaY = maxYSpritePosition - minYSpritePosition;
        currentFriction = normalFriction;
    }

    private void Update()
    {
        ShadowRender();

        if (!col.enabled && sprite.localPosition.y < minYSpritePosition + 0.5f)
            col.enabled = true;
    }

    private void FixedUpdate()
    {
        if (canMove)
            ActionHandle();

        FrictionSimulation();
    }

    protected virtual bool FacingToDirection(float xDirection)
    {
        SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        sprite.GetComponent<SpriteRenderer>().flipX = xDirection < 0;
        return !spriteRenderer.flipX;
    }

    private void ShadowRender()
    {
        float deltaY = sprite.localPosition.y - minYSpritePosition;
        float shadowScale = maxShadowScale - deltaY / maxDeltaY * maxShadowScale;
        shadowScale = Mathf.Max(shadowScale, 0.3f);

        shadow.localScale = new Vector3(shadowScale, shadowScale, shadow.localScale.z);
    }

    protected abstract void ActionHandle();

    public virtual void OnEndSpawn()
    {
        EnableMove();
    }

    private void EnableMove()
    {
        canMove = true;
        col.enabled = true;
        CanAttack = true;
    }

    private void FrictionSimulation()
    {
        if (isFrictionEnable)
        {
            rb.velocity = new Vector2(
                    Mathf.Lerp(rb.velocity.x, 0, currentFriction),
                    Mathf.Lerp(rb.velocity.y, 0, currentFriction));

            if (IsVelocityEqualZero())
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Transform other = collision.transform;
        if (other.CompareTag("Dice"))
            OnCollisionWithDice(collision);
        else
            OnCollisionWithOther(collision);
    }

    protected virtual void OnCollisionWithDice(Collision2D dice) { }
    protected virtual void OnCollisionWithOther(Collision2D other) { }

    public void TakeHit(int diceNumber)
    {
        if (dieCondition == null ||
            dieCondition.CheckDieCondition(diceNumber))
            Die();
        else
            Hit();
    }

    protected virtual void Hit()
    {
        animator.SetTrigger("hit");
    }

    public virtual void Die()
    {
        if (disabledDie) return;

        CanAttack = false;
        canMove = false;
        animator.SetTrigger("die");
        disabledDie = true;
    }


    protected bool IsVelocityEqualZero()
    {
        float xVelocityAbs = Mathf.Abs(rb.velocity.x);
        float yVelocityAbs = Mathf.Abs(rb.velocity.y);

        return xVelocityAbs < 0.01f && yVelocityAbs < 0.01f;
    }

    public IEnumerator Disappear(float duration)
    {
        float scale = transform.localScale.x * 1.1f;
        transform.localScale = new Vector3(scale, scale, 1);
        yield return new WaitForSeconds(0.05f);

        float elapsedTime = 0;
        Vector3 originalScale = transform.localScale;

        Vector3 newScale = new(originalScale.x * 0.5f * Mathf.Sign(transform.localScale.x),
            originalScale.y * 0.5f, 1);

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, newScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void OnHitGround()
    {
        CameraController.instance.StartShake(rb.mass * 0.007f, rb.mass * 0.07f);
    }

    //private IEnumerator FallDown(float startFallHeight, float duration)
    //{
    //    float time = 0f;
    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float ratio = Mathf.Clamp01(time / duration);
    //        float yPosition = Mathf.Lerp(startFallHeight, minYSpritePosition, ratio);
    //        sprite.localPosition = new Vector3(sprite.localPosition.x, yPosition, sprite.localPosition.z);

    //        yield return new WaitForEndOfFrame();
    //    }
    //    sprite.localPosition = new Vector3(sprite.localPosition.x, minYSpritePosition, sprite.localPosition.z);
    //    OnEndSpawn();
    //}
}
