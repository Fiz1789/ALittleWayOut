using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using KBCore.Refs;

namespace Platformer
{
    public class SpawnObby : MonoBehaviour


    {
        [Header("Camera Shake (Cinemachine)")]
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private float shakeIntensity = 3f;
        CinemachineBasicMultiChannelPerlin noise;

        [Header("Trigger Settings")]
        [SerializeField] private bool triggerOnPlayer = false;
        [SerializeField] private bool animateSimultaneouslyOnTrigger = false;

        [Header("References")]
        [SerializeField] private Transform[] targetObjects;
        [SerializeField] private float moveDistance = 5f;
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private PlayerController player; // Assign in inspector

        private bool hasStarted = false;

        [Header("Options")]
        [SerializeField] private bool reenablePlayerAfter = true;
        void Awake()
        {
            if (cinemachineCamera != null)
            {
                noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
                if (noise == null)
                {
                    Debug.LogError("CinemachineBasicMultiChannelPerlin missing! Add it as a compo   nent to the CinemachineCamera.");
                }
            }
            else
            {
                Debug.LogError("No CinemachineCamera assigned!");
            }
        }
        private void OnTriggerEnter(Collider other)

        {
            if (!triggerOnPlayer) return;
            if (player != null && other.gameObject == player.gameObject)
            {
                if (!hasStarted)
                {
                    hasStarted = true;
                    StartMoveIn(animateSimultaneouslyOnTrigger);
                }
            }
        }
        public void StartMoveIn(bool animateSimultaneously = false)
        {
            if (animateSimultaneously)
                StartCoroutine(MoveAllInSimultaneously());
            else
                StartCoroutine(MoveAllInSequence());
        }

        private IEnumerator MoveAllInSequence()
        {
            // Disable player movement
            if (player != null)
            {
                player.MovementEnabled = false;
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }

            AudioManager.Instance.PlaySpawnObby();
            // Camera shake for moveDuration
            Coroutine shakeCoroutine = null;
            if (noise != null)
            {
                shakeCoroutine = StartCoroutine(CameraShakeForDuration(moveDuration));
            }

            foreach (var obj in targetObjects)
            {
                yield return StartCoroutine(MoveSingleObjectIn(obj));
            }

            if (shakeCoroutine != null)
                yield return shakeCoroutine;

            // Re-enable player movement
            if (player != null && reenablePlayerAfter)
            {
                player.MovementEnabled = true;
            }
        }

        private IEnumerator MoveAllInSimultaneously()
        {
            // Disable player movement
            if (player != null)
            {
                player.MovementEnabled = false;
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }

            // Camera shake for moveDuration
            Coroutine shakeCoroutine = null;
            if (noise != null)
            {
                shakeCoroutine = StartCoroutine(CameraShakeForDuration(moveDuration));
            }

            Coroutine[] coroutines = new Coroutine[targetObjects.Length];
            for (int i = 0; i < targetObjects.Length; i++)
            {
                coroutines[i] = StartCoroutine(MoveSingleObjectIn(targetObjects[i]));
            }
            foreach (var c in coroutines)
            {
                yield return c;
            }

            if (shakeCoroutine != null)
                yield return shakeCoroutine;

            // Re-enable player movement
            if (player != null && reenablePlayerAfter)
            {
                player.MovementEnabled = true;
            }
        }

        private IEnumerator CameraShakeForDuration(float duration)
        {
            if (noise == null)
            {
                Debug.LogWarning("[SpawnObby] Camera shake requested but noise is null.");
                yield break;
            }
            Debug.Log($"[SpawnObby] Camera shake started for {duration} seconds, intensity {shakeIntensity}.");
            noise.AmplitudeGain = shakeIntensity;
            yield return new WaitForSeconds(duration);
            noise.AmplitudeGain = 0f;
            Debug.Log("[SpawnObby] Camera shake ended.");
        }

        private IEnumerator MoveSingleObjectIn(Transform targetObject)
        {
            if (targetObject == null) yield break;
            Vector3 startLocalPos = targetObject.localPosition;
            Vector3 endLocalPos = startLocalPos + Vector3.right * moveDistance;

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                targetObject.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, t);
                yield return null;
            }
            targetObject.localPosition = endLocalPos;
        }
    }
}