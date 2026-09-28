using UnityEngine;
using Unity.Cinemachine;
using KBCore.Refs;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class BrokenFloor : MonoBehaviour


    {

        [Header("Scene Transition")]
        [SerializeField,Anywhere] SceneTransition sceneTransition;
        [Header("Floor References")]
        [SerializeField, Child] GameObject notBrokenFloor;
        [SerializeField, Child] GameObject brokenFloor;

        [Header("Player Reference")]
        [SerializeField, Scene] PlayerController player;

        [Header("Camera Shake (Cinemachine)")]
        [SerializeField, Scene] CinemachineCamera cinemachineCamera;
        [SerializeField] float shakeIntensity = 3f;
        [SerializeField] float shakeDuration = 0.5f;
        CinemachineBasicMultiChannelPerlin noise;

        [Header("Break Settings")]
        [SerializeField] float breakForce = 10f;


        [SerializeField, Anywhere] Rigidbody playerRb;

        [SerializeField, Anywhere] Animator playerAnimator;

        [Header("Fall Collider")]
        [SerializeField, Anywhere] Collider fallCollider;
        bool hasBroken = false;
        void Awake()
        {
            if (cinemachineCamera != null)
            {
                noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
                if (noise == null)
                {
                    Debug.LogError("CinemachineBasicMultiChannelPerlin missing! Add it as a component to the CinemachineCamera.");
                }
            }
            else
            {
                Debug.LogError("No CinemachineCamera assigned!");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (hasBroken) return;
            if (!other.CompareTag("Player")) return;

            StartCoroutine(BreakSequence());
        }

        IEnumerator BreakSequence()
        {
            hasBroken = true;

            // Disable input (NOT physics)
            player.MovementEnabled = false;
            player.ForceExitPushState();

            // Stop horizontal motion only (lock X and Z, keep Y for gravity/falling)
            playerRb.linearVelocity = new Vector3(0f, playerRb.linearVelocity.y, 0f);

            AudioManager.Instance.PlayBreakingFloor();
            // Look-around animation
            playerAnimator.SetTrigger("NervousLookAround");
            yield return new WaitForSeconds(1.5f);


            // Camera shake
            if (noise != null)
            {
                noise.AmplitudeGain = shakeIntensity;
                yield return new WaitForSeconds(shakeDuration);
                noise.AmplitudeGain = 0f;
            }

            playerAnimator.SetTrigger("Falling");
            yield return new WaitForSeconds(0.5f);
            // Break floor
            BreakFloor();

            // Disable fall collider so player can fall through
            if (fallCollider != null)
                fallCollider.enabled = false;


            // Increase player's downward velocity for faster fall
            playerRb.linearVelocity = new Vector3(0f, -20f, 0f);

            player.MovementEnabled = true;
            // Fall animation

            yield return new WaitForSeconds(3f);
            Debug.Log("Player fell through broken floor, transitioning to PuzzleScene.");
            // Fade in before loading the scene
            if (sceneTransition != null)
            {
                Debug.Log("Fading in before scene transition.");
                yield return sceneTransition.FadeOut(1f);
            }
            SceneManager.LoadScene("PuzzleScene");

        }

        void BreakFloor()
        {
            notBrokenFloor.SetActive(false);
            brokenFloor.SetActive(true);

            foreach (Rigidbody rb in brokenFloor.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
                rb.AddForce(Vector3.down * breakForce, ForceMode.VelocityChange);
            }

            Destroy(brokenFloor, 5f);
        }
    }
}
