using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;

namespace Platformer
{
    public static class CheckpointManager
    {
        static Vector3 lastCheckpoint = Vector3.zero;
        public static bool HasCheckpoint { get; private set; } = false;

        // Store touched zone IDs
        static HashSet<string> touchedZones = new HashSet<string>();

        // Call this when a zone is touched
        public static void MarkZoneAsTouched(string zoneId)
        {
            touchedZones.Add(zoneId);
        }

        // Returns true if the zone has been touched
        public static bool HasCheckpointForZone(string zoneId)
        {
            return touchedZones.Contains(zoneId);
        }

        public static void SetCheckpoint(Vector3 position)
        {
            lastCheckpoint = position;
            HasCheckpoint = true;
            Debug.Log($"Checkpoint set: {position}");
        }

        public static Vector3 GetCheckpoint()
        {
            return lastCheckpoint;
        }
    }
}
