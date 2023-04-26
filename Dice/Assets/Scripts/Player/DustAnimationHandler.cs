using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DustAnimationHandler : MonoBehaviour
    {
        private void Destroy()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}