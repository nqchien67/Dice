using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;

        public float camSmoothing = 0.2f;
        public float rotationMultiplier = 7.5f;

        private Vector3 currentVelocity = Vector3.zero;
        private float shakeTimeRemaining;
        private float shakePower;
        private float shakeRotation;
        private float shakeFadeTime;

        private Vector3 originPos;

        private Transform target;

        private void Start()
        {
            originPos = transform.position;
            instance = this;
        }

        private void Update()
        {
            if (target != null)
            {
                Vector3 targetPosition = target.position;
                targetPosition.z = transform.position.z;
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, camSmoothing);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                Screen.fullScreen = !Screen.fullScreen;
            }
        }

        private void LateUpdate()
        {
            if (shakeTimeRemaining > 0)
            {
                shakeTimeRemaining -= Time.deltaTime;

                float xAmount = Random.Range(-1f, 1f) * shakePower;
                float yAmount = Random.Range(-1f, 1f) * shakePower;

                transform.position += new Vector3(xAmount, yAmount, 0f);

                shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);
                shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);
            }
            else
            {
                if (target == null)
                    transform.position = originPos;
            }

            transform.rotation = Quaternion.Euler(0f, 0f, shakeRotation * Random.Range(-1f, 1f));
        }

        public void StartShake(float length, float shakePower)
        {
            if (shakePower > this.shakePower)
            {
                shakeTimeRemaining = length;
                this.shakePower = shakePower;
                shakeFadeTime = shakePower / length;
                shakeRotation = shakePower * rotationMultiplier;
            }
        }

        public void StartFollow()
        {
            target = GameObject.FindWithTag("Player").transform;
        }
    }
}