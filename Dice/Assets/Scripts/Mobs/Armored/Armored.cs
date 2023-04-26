using Assets.Scripts.System.Wave;
using UnityEngine;

public abstract class Armored : MobController
{
    public float moveForce;

    protected abstract override void ActionHandle();

    protected void MoveToDirection(Vector2 direction)
    {
        rb.AddForce(direction * moveForce, ForceMode2D.Impulse);
    }

    protected override bool FacingToDirection(float xDirection)
    {
        bool isFacingRight = xDirection > 0;

        float newX = isFacingRight ? 1 : -1;
        transform.localScale = new Vector3(newX, 1, 1);
        transform.GetChild(2).localScale = new Vector3(newX, 1, 1);

        return isFacingRight;
    }

    protected void SetAnimatorMoving(bool isMoving)
    {
        foreach (var anim in GetComponentsInChildren<Animator>())
        {
            anim.SetBool("isMoving", isMoving);
        }
    }

    protected override void Hit()
    {
        foreach (var anim in GetComponentsInChildren<Animator>())
        {
            anim.SetTrigger("hit");
        }
    }

    public override void Die()
    {
        if (transform.childCount > 3)
        {
            Hit();
            Destroy(transform.GetChild(transform.childCount - 1).gameObject);
        }
        else
        {
            base.Die();
            WaveManager.instance.DefeatedABoss();
        }
    }
}
