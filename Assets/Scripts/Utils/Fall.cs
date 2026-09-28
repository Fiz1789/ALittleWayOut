using UnityEngine;
using KBCore.Refs;

namespace Platformer {
    public class Fall : MonoBehaviour {

        [SerializeField,Anywhere] PlayerHealth PlayerHealth;
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            // Only teleport if player is alive
            if (PlayerHealth.currentHealth > 1 && CheckpointManager.HasCheckpoint)
            {
                var cp = CheckpointManager.GetCheckpoint();
                other.transform.position = cp + Vector3.up * 0.5f;
                var rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                PlayerHealth.PlayerDamage(1);
            }
            else if (PlayerHealth.currentHealth == 1){
                PlayerHealth.PlayerDamage(1);
            }
            else if (PlayerHealth.currentHealth > 0)
            {
                Debug.LogWarning("No checkpoint set. Player not respawned.");
            }
            // If health is 0, do nothing (death logic handled elsewhere)
        }
    }
}