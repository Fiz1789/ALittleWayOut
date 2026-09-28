using System.Collections;
using UnityEngine;

namespace Platformer
{
    public static class KeySpawnSequenceUtility
    {
        public static IEnumerator PlayKeySpawnSequence(GameObject key, float moveDownDuration, float moveDownDistance, PlayerController player = null)
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
            key.SetActive(true);
            // Move key down
            Vector3 startPos = key.transform.position;
            Vector3 endPos = startPos + Vector3.down * moveDownDistance;
            float elapsedTime = 0f;

            while (elapsedTime < moveDownDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / moveDownDuration;
                key.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            key.transform.position = endPos;

            // Enable rotation on SimpleGemsAnim
            SimpleGemsAnim gemsAnim = key.GetComponent<SimpleGemsAnim>();
            if (gemsAnim != null)
            {
                gemsAnim.isRotating = true;
            }

            // Re-enable player movement
            if (player != null)
            {
                player.MovementEnabled = true;
            }

            Debug.Log("Key animation complete!");
        }
    }
}
