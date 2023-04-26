using UnityEngine;

namespace Assets.Scripts.AfterWin
{
    public class GateAnimEvenHandler : MonoBehaviour
    {
        private void EndOpenGate()
        {
            GetComponentInParent<Gate>().OnEndOpenGate();
        }
    }
}