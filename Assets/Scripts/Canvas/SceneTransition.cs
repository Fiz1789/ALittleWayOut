using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using KBCore.Refs;

namespace Platformer
{
    public class SceneTransition : MonoBehaviour
    {
        public GameObject fadingPanel;
        public Image fadePanel;

        [SerializeField] private ClarifyText clarifyText;

        void Start()
        {
            StartCoroutine(FadeIn(3f));
            // if (fadePanel != null)
            //     fadePanel.gameObject.SetActive(false);
        }

        void PlaySceneMusic()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (AudioManager.Instance != null)
            {
                if (sceneName == "MainMenu" && AudioManager.Instance.backgroundMusic != null)
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.backgroundMusic);
                else
                if (sceneName == "TutorialScene" && AudioManager.Instance.tutorialMusic != null)
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.tutorialMusic);
                else if (sceneName == "PuzzleScene" && AudioManager.Instance.puzzleMusic != null)
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.puzzleMusic);

            }
        }
        public void TriggerFadeOut(float duration = 1f)
        {
            StartCoroutine(FadeOut(duration));
        }

        public void TriggerFadeIn(float duration = 1f)
        {
            StartCoroutine(FadeIn(duration));
        }
        public IEnumerator FadeIn(float duration = 1f)
        {
            if (fadePanel != null)
                fadePanel.gameObject.SetActive(true);
            Color panelColor = fadePanel.color;
            fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 1);
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(1, 0, t / duration);
                fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, alpha);
                yield return null;
            }
            fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 0);
            fadePanel.gameObject.SetActive(false);
            yield return new WaitForSeconds(2f);
            PlaySceneMusic();
        }

        // Fade out: black panel alpha 0 -> 1
        public IEnumerator FadeOut(float duration = 1f)
        {
            if (fadePanel != null)
                fadePanel.gameObject.SetActive(true);
            Color panelColor = fadePanel.color;
            fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 0);
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(0, 1, t / duration);
                fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, alpha);
                yield return null;
            }
            fadePanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 1);
        }

        // Fade out, then call ClarifyText for death text
        public IEnumerator FadeOutDeath(float fadeOutDuration = 1f)
        {
            yield return StartCoroutine(FadeOut(fadeOutDuration));
            if (clarifyText != null)
                clarifyText.AnimateText();
        }
    }
}