using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.AfterWin
{
    public class Trap : MonoBehaviour
    {
        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            if (other.CompareTag("Hurtbox"))
            {
                anim.SetTrigger("spike");

                Vector2 kbDir = transform.position - other.transform.position;
                Vector2 clampDir = new Vector2(kbDir.x, Mathf.Min(kbDir.y, -1));

                other.GetComponent<Hurtbox>().Hit(clampDir.normalized, false);
            }
        }
    }
}