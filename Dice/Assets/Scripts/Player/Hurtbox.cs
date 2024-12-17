using System;
using Assets.Scripts.Dice;
using Assets.Scripts.System;
using UnityEngine;

namespace Player
{
    public class Hurtbox : MonoBehaviour
    {
        private PlayerController dad;
        public Transform Dice { get; private set; }
        private Animator anim;

        private void Awake()
        {
            dad = transform.parent.GetComponent<PlayerController>();
            anim = dad.GetComponentInChildren<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TriggerHandler(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TriggerHandler(other);
        }

        private void TriggerHandler(Collider2D other)
        {
            if (!dad.IsKnockbacking &&
            !dad.IsHoldingDice &&
            other.CompareTag("Dice"))
            {
                DiceController dice = other.GetComponent<DiceController>();
                if (dice.TryPickUp())
                {
                    dad.IsHoldingDice = true;
                    anim.SetBool("isHoldingDice", true);

                    Dice = dice.transform;

                    try { InstructionText.instance.OnIndex2(); }
                    catch (NullReferenceException exception) { Debug.LogWarning(exception); }
                }
            }
            else
                HandleHitCollision(other);
        }

        private void HandleHitCollision(Collider2D other)
        {
            if (other.CompareTag("Mob"))
            {
                MobController mob = other.GetComponent<MobController>();
                if (mob.CanAttack)
                    Hit((dad.transform.position - mob.transform.position).normalized);
            }
        }

        public bool Hit(Vector2 knockbackDirection, bool canAvoid = true)
        {
            return dad.Hit(knockbackDirection, canAvoid);
        }
    }
}