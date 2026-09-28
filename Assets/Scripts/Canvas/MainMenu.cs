using UnityEngine;
using KBCore.Refs;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

namespace Platformer
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject mainMenuPanel;
        [SerializeField] GameObject settingsPanel;
        [SerializeField] Slider musicVolumeSlider;

        [Header("Cameras")]
        [SerializeField] GameObject mainMenuCamera;
        [SerializeField] GameObject introCamera;

        [Header("Cutscene")]
        [SerializeField] PlayableDirector introTimeline;
        void Start()
        {
            // Ensure the game starts at normal time scale
            Time.timeScale = 1f;
            musicVolumeSlider.value = AudioManager.Instance.backgroundMusicVolume;
            mainMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);

            mainMenuCamera.SetActive(true);
            introCamera.SetActive(false);
        }

        public void StartGame()
        {
            StartCoroutine(LoadPlaySceneWithDelay("TutorialScene", 0.15f));
        }

        public void QuitGame()
        {
            StartCoroutine(QuitGameWithDelay(0.15f));
        }

        private IEnumerator QuitGameWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Application.Quit();
        }
        private IEnumerator LoadPlaySceneWithDelay(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            mainMenuPanel.SetActive(false);
            if (mainMenuCamera != null)
                mainMenuCamera.SetActive(false);

            if (introCamera != null)
                introCamera.SetActive(true);

            if (introTimeline != null)
            {
                introTimeline.time = 0;
                introTimeline.Play();
                yield return new WaitForSeconds((float)introTimeline.duration);
            }
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopMusic();

            mainMenuPanel.SetActive(false);
            SceneManager.LoadScene(sceneName);
        }


        private IEnumerator ShowSettingsPanelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        private IEnumerator ShowMainMenuPanelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            mainMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);
        }


        public void BackButton()
        {
            StartCoroutine(ShowMainMenuPanelAfterDelay(0.15f));
        }

        public void SettingsButton()
        {
            StartCoroutine(ShowSettingsPanelAfterDelay(0.15f));
        }

    }
}