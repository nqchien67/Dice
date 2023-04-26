using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.System.Health
{
    public class HealthBar : MonoBehaviour
    {
        public GameObject healthPrefab;

        public CanvasGroup blackScreen;
        public float fadeTime;

        public static HealthBar instance;

        private Vector2[] healthPositions;
        private bool gameOver = false;

        private void Start()
        {
            int childCount = transform.childCount;
            healthPositions = new Vector2[childCount];
            for (int i = 0; i < childCount; i++)
                healthPositions[i] = transform.GetChild(i).localPosition;

            instance = this;
        }

        public int GetCurrentHealth()
        {
            return transform.childCount;
        }

        public void LoseHealth()
        {
            if (GetCurrentHealth() > 0)
            {
                Transform child = transform.GetChild(transform.childCount - 1);
                child.GetComponentInChildren<Animator>().SetTrigger("lose");
                child.parent = null;

                if (GetCurrentHealth() < 1)
                {
                    Dead();
                }
            }
        }

        public void GainHealth()
        {
            Vector2 newHPPosition = healthPositions[transform.childCount];
            GameObject newHP = Instantiate(healthPrefab, transform);
            newHP.transform.localPosition = newHPPosition;
        }

        private void Dead()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                blackScreen.alpha = elapsedTime / fadeTime;
                yield return null;
                elapsedTime += Time.deltaTime;
            }
            blackScreen.alpha = 1;

            InstructionText.instance.SetNextText("Press R to restart");
            gameOver = true;

            yield return null;
        }

        private void Update()
        {
            if (gameOver && Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(gameObject.scene.name);
            }
        }
    }
}