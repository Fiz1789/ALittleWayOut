using UnityEngine;
using KBCore.Refs;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField, Anywhere] private GameObject pauseMenuPanel;

        [SerializeField, Anywhere] private GameObject settingsPanel;
        [SerializeField] private Slider musicVolumeSlider;


        void Start()
        {
            pauseMenuPanel.SetActive(false);
            if (musicVolumeSlider != null && AudioManager.Instance != null)
                musicVolumeSlider.value = AudioManager.Instance.backgroundMusicVolume; // or .backgroundMusicVolume if you renamed it
        }
        void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (Time.timeScale == 1f)
                {
                    PauseGame();
                }
                else if (Time.timeScale == 0f)
                {
                    ResumeGame();
                }
            }
        }
        public void ResumeGame()
        {
            StartCoroutine(ResumeGameWithDelay(0.15f));
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            pauseMenuPanel.SetActive(true);
        }

        public void QuitToMainMenu()
        {
            StartCoroutine(QuitToMainMenuWithDelay("MainMenu", 0.15f));
        }

        public void OpenSettingsMenu()
        {
            StartCoroutine(SettingsMenuWithDelay(0.15f));
        }

        public void BackToPauseMenu()
        {
            StartCoroutine(BackToPauseMenuWithDelay(0.15f));
        }

        private IEnumerator ResumeGameWithDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            Time.timeScale = 1f;
            pauseMenuPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
        private IEnumerator QuitToMainMenuWithDelay(string sceneName, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            Time.timeScale = 1f;
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopMusic();

            pauseMenuPanel.SetActive(false);
            settingsPanel.SetActive(false);
            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator SettingsMenuWithDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            pauseMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        private IEnumerator BackToPauseMenuWithDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            pauseMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }
    }
}