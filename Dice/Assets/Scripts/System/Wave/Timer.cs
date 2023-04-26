using TMPro;
using UnityEngine;

namespace Assets.Scripts.System.Wave
{
    public class Timer : MonoBehaviour
    {
        private TMP_Text text;
        public int currentSecond = 0;
        private float timeFlag;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            timeFlag = Time.time;
        }

        private void Update()
        {
            if (currentSecond <= 0)
                text.text = "";
            else if (Time.time - timeFlag > 1)
            {
                currentSecond--;

                text.color = currentSecond > 5 ? Color.white : Color.red;
                text.text = currentSecond.ToString();

                timeFlag = Time.time;
            }
        }

        public void AddTime(int second)
        {
            currentSecond = Mathf.Min(currentSecond + second, 40);
        }
    }
}