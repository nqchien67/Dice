using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Mobs
{
    public class Arrow : MonoBehaviour
    {
        public float shootForce;
        public Vector2 direction;

        private Rigidbody2D rb;
        private Collider2D col;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
        }

        private void Start()
        {
            rb.AddForce(direction * shootForce, ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Hurtbox") || other.CompareTag("Walls") || other.CompareTag("Dice"))
            {
                bool stick = true;
                if (other.TryGetComponent<Hurtbox>(out var player))
                    stick = player.Hit(direction.normalized);

                if (stick)
                {
                    StickToTarget(other.transform);
                    Destroy(gameObject, 6);
                }
            }
        }

        private void StickToTarget(Transform target)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
            col.enabled = false;

            transform.parent = target;
            GetComponent<SpriteRenderer>().sortingLayerName = "DroppedWeapon";
        }
    }
}