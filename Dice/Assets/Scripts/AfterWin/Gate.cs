using UnityEngine;

namespace Assets.Scripts.AfterWin
{
    public class Gate : MonoBehaviour
    {
        public void Open()
        {
            GetComponentInChildren<Animator>().SetTrigger("open");
        }

        public void OnEndOpenGate()
        {
            GetComponent<PolygonCollider2D>().enabled = false;
            CameraController.instance.StartFollow();
        }
    }
}