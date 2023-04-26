using UnityEngine;

namespace Assets.Scripts.System.Health
{
    public class HealPointAnimEventHandler : MonoBehaviour
    {
        private void OnLose()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}