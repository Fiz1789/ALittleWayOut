using KBCore.Refs;
using UnityEngine;
using System.Collections;
using Platformer;

namespace Platformer
{
    public class HolePuzzleWall : ValidatedMonoBehaviour
    {

        [Header("Puzzle References")]
        [SerializeField, Anywhere] private GameObject[] correctObject;
        [SerializeField, Anywhere] private GameObject _keyPrefab;
        [SerializeField, Anywhere] private Transform _spawnPoint;
        // [SerializeField, Anywhere] private AudioSource _soundEffect;
        [SerializeField, Scene] private PlayerController _player;

        [Header("Key Animation Settings")]
        [SerializeField] private float _moveDownDuration = 2f;
        [SerializeField] private float _moveDownDistance = 3f;

        private bool _hasSpawned = false;

        void OnTriggerEnter(Collider other)
        {
            if (System.Array.Exists(correctObject, obj => obj == other.gameObject) && !_hasSpawned)
            {
                // Change tag to prevent further push detection
                other.gameObject.tag = "Untagged";
                // Force player out of push state immediately
                _player?.ForceExitPushState();
                SpawnKey();
            }
        }

        void SpawnKey()
        {
            if (_keyPrefab != null)
            {
                Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position + Vector3.up;
                GameObject newKey = Instantiate(_keyPrefab, spawnPos, Quaternion.identity);
                StartCoroutine(KeySpawnSequenceUtility.PlayKeySpawnSequence(newKey, _moveDownDuration, _moveDownDistance, _player));
                _hasSpawned = true;
            }
        }

    //     IEnumerator KeySpawnSequence(GameObject key)
    //     {
    //         // Disable player movement using MovementEnabled property
    //         if (_player != null)
    //         {
    //             _player.MovementEnabled = false;
    //             Rigidbody rb = _player.GetComponent<Rigidbody>();
    //             if (rb != null)
    //             {
    //                 rb.linearVelocity = Vector3.zero;
    //                 rb.angularVelocity = Vector3.zero;
    //             }
    //         }

    //         // Play sound effect
    //         // if (_soundEffect != null)
    //         // {
    //         //     _soundEffect.Play();
    //         // }

    //         // Move key down
    //         Vector3 startPos = key.transform.position;
    //         Vector3 endPos = startPos + Vector3.down * _moveDownDistance;
    //         float elapsedTime = 0f;

    //         while (elapsedTime < _moveDownDuration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float t = elapsedTime / _moveDownDuration;
    //             key.transform.position = Vector3.Lerp(startPos, endPos, t);
    //             yield return null;
    //         }

    //         key.transform.position = endPos;

    //         // Enable rotation on SimpleGemsAnim
    //         SimpleGemsAnim gemsAnim = key.GetComponent<SimpleGemsAnim>();
    //         if (gemsAnim != null)
    //         {
    //             gemsAnim.isRotating = true;
    //         }

    //         // Re-enable player movement
    //         if (_player != null)
    //         {
    //             _player.MovementEnabled = true;
    //         }

    //         Debug.Log("Key animation complete!");
    //     }
    }
    // [Header("Puzzle References")]
    // [SerializeField,Anywhere] GameObject squareObject; // The specific square object to detect
    // [SerializeField,Anywhere] GameObject keyPrefab; // The key prefab to spawn
    // [SerializeField,Anywhere] Transform spawnPoint; // Where to spawn the key

    // private bool hasSpawned = false; // Prevent multiple spawns

    // void OnTriggerEnter(Collider other)
    // {
    //     // Check if it's the specific square object
    //     if (other.gameObject == squareObject && !hasSpawned)
    //     {
    //         SpawnKey();
    //     }
    // }

    // void SpawnKey()
    // {
    //     if (keyPrefab != null)
    //     {
    //         Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + Vector3.up;
    //         Instantiate(keyPrefab, spawnPos, Quaternion.identity);
    //         hasSpawned = true;
    //         Debug.Log("Key spawned!");
    //     }
    // }
}
