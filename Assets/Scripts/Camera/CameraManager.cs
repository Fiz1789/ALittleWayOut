using UnityEngine;
using Unity.Cinemachine;
using KBCore.Refs;

namespace Platformer
{
    public class CameraManager : MonoBehaviour
    {

        [Header("Camera Pair")]
        [SerializeField] private CinemachineCamera originalCamera; // e.g., vCam_ZoneA
        [SerializeField] private CinemachineCamera targetCamera;    // e.g., vCam_ZoneB

        [Header("Safety")]
        [SerializeField] private float switchCooldown = 1f;
        [SerializeField] private string playerTag = "Player";

        private float lastSwitchTime;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag))
                return;

            if (Time.time - lastSwitchTime < switchCooldown)
                return;

            originalCamera.Priority = 0;
            targetCamera.Priority = 10;

            lastSwitchTime = Time.time;
            // Save this trigger as a checkpoint
            CheckpointManager.SetCheckpoint(transform.position);
        }
    }
}

