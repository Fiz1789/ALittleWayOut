using UnityEngine;
using System.Collections.Generic;

namespace Platformer
{
    public class SkipPuzzle : MonoBehaviour
    {
        [SerializeField] private List<Transform> zoneTriggers;
        [SerializeField] private PlayerController player;
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                SkipToNextZone();
            }
        }

        void SkipToNextZone()
        {
            foreach (var zone in zoneTriggers)
            {
                if (!CheckpointManager.HasCheckpointForZone(zone.name))
                {
                    player.transform.position = zone.position;
                    // Mark this zone as touched
                    CheckpointManager.MarkZoneAsTouched(zone.name);
                    // Optionally: Set checkpoint for this zone
                    CheckpointManager.SetCheckpoint(zone.position);
                    break;
                }
            }
        }
    }
}