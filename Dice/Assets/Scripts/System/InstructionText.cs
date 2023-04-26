using Assets.Scripts.System.Wave;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class InstructionText : MonoBehaviour
    {
        public static InstructionText instance;

        public GameObject dicePrefab;
        public GameObject testWarlock;
        public GameObject slimePrefab;

        private TMP_Text text;
        private Animator animator;

        private int currentIndex = 0;
        private string currentText;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            currentText = "You can move with [WASD]";
            ShowNewText();
            instance = this;
        }

        private void Update()
        {
            if (currentIndex == 0 &&
                (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            {
                SetNextText("Press [Space] to roll");
            }

            if (currentIndex == 1 && Input.GetKeyDown(KeyCode.Space))
            {
                SetNextText("Move over a dice to grab it");
                Instantiate(dicePrefab, new Vector2(-2, -0.5f), Quaternion.identity);
            }

            if ((currentIndex == 4 || currentIndex == 5) && GameObject.FindWithTag("Mob") == null)
            {
                SetNextText("If all dice match with at least one other, you get a bonus");
                Instantiate(dicePrefab, new Vector2(0, -1), Quaternion.identity);
                WaveManager.instance.StartSpawn();
            }
        }

        public void SetNextText(string text)
        {

            currentText = text;
            currentIndex++;
            animator.SetTrigger("out");
        }

        private void ShowNewText()
        {
            text.text = currentText;
        }

        public void OnIndex2()
        {
            if (currentIndex == 2)
            {
                Instantiate(testWarlock, new Vector2(3f, 0), Quaternion.identity);
                SetNextText("[Click] to throw it at enemies");
            }
        }

        public void OnIndex3()
        {
            if (currentIndex == 3)
            {
                SetNextText("Enemies will have some condition the dice must meet");

                GameObject slime = Instantiate(slimePrefab, new Vector2(2, -1), Quaternion.identity);

                void action()
                {
                    string text = "";
                    if (slime != null)
                    {
                        string slimeVariant = slime.GetComponentInChildren<TextMeshPro>().text;
                        text = $"For example, only {slimeVariant} number can hurt this one";
                    }

                    SetNextText(text);
                }

                StartCoroutine(DelayCall(action, 4));
            }
        }

        public void OnIndex6()
        {
            if (currentIndex == 6)
            {
                SetNextText("");
            }
        }

        public void Victory()
        {
            SetNextText("You won! Now go get the cake.");
        }

        private IEnumerator DelayCall(Action action, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            action();
        }
    }
}