using UnityEngine;

namespace Assets.Scripts.System.Health
{
    public class HealthPoint : MonoBehaviour
    {
        private Transform sprite;
        private Transform shadow;

        private float maxShadowScale;
        private float spriteShadowRatio;
        private float originSpriteX;

        private void Awake()
        {
            sprite = transform.GetChild(0);
            shadow = transform.GetChild(1);
        }

        private void Start()
        {
            originSpriteX = sprite.localPosition.x;
            spriteShadowRatio = shadow.localScale.x / transform.GetChild(0).localScale.x;
        }

        private void Update()
        {
            RenderShadow();
        }

        private void RenderShadow()
        {
            if (sprite.localScale.x != 1)
            {
                float newScale = sprite.localScale.x * spriteShadowRatio;
                shadow.localScale = new Vector3(newScale, newScale, 1);
            }

            if (sprite.localPosition.x != originSpriteX)
            {
                float deltaX = sprite.localPosition.x - originSpriteX;
                shadow.localPosition = new Vector2(deltaX, shadow.localPosition.y);
            }
        }
    }
}