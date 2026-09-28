using UnityEngine;
using KBCore.Refs;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Platformer
{
    public class EndingTriggerZone : MonoBehaviour
    {
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] public GameObject endingCanvas;

        [SerializeField, Anywhere] public PlayerController playerController;
        [SerializeField] private float fadeOutDuration = 1f;
        [SerializeField] private float textWaitDuration = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (playerController != null)
                    playerController.MovementEnabled = false;
                if (endingCanvas != null)
                    StartCoroutine(EndSequence());

                // Optionally, disable this collider so it only triggers once
                GetComponent<Collider>().enabled = false;

            }
        }
        private IEnumerator EndSequence()
        {
            yield return sceneTransition.FadeOutDeath(fadeOutDuration);
            yield return new WaitForSeconds(textWaitDuration);
            SceneManager.LoadScene("MainMenu"); // Use your main menu scene name
        }
    }
}