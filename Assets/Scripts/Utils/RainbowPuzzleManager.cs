using UnityEngine;
using System.Collections.Generic;
using KBCore.Refs;

namespace Platformer
{
    public class RainbowPuzzleManager : ValidatedMonoBehaviour
    {
        [Header("Rainbow Platforms (Order: Red, Orange, Yellow, Green, Blue, Indigo, Violet)")]
        [SerializeField, Child] private RainbowPlatform[] platforms = new RainbowPlatform[7];

        [SerializeField, Anywhere] private GameObject _keyPrefab;
        [SerializeField, Anywhere] private Transform _spawnPoint;
        [SerializeField, Scene] private PlayerController _player;
        // [SerializeField, Anywhere] private AudioSource _soundEffect;

        private bool[] correctStates = new bool[7];
        private bool puzzleSolved = false;

        public void NotifyPlatform(RainbowPlatform platform, bool isCorrect)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i] == platform)
                {
                    correctStates[i] = isCorrect;
                    break;
                }
            }
            CheckPuzzle();
        }

        private void CheckPuzzle()
        {
            if (puzzleSolved) return;
            for (int i = 0; i < correctStates.Length; i++)
            {
                if (!correctStates[i])
                    return;
            }
            puzzleSolved = true;
            OnPuzzleSolved();
        }

        private void OnPuzzleSolved()
        {
            Debug.Log("Rainbow puzzle solved!");
            SpawnKey();
        }
        void SpawnKey()
        {

            // Untag all colored objects on platforms
            foreach (var platform in platforms)
            {
                Collider[] colliders = Physics.OverlapBox(platform.transform.position, platform.transform.localScale / 2f);
                foreach (var col in colliders)
                {
                    ColoredObject colored = col.GetComponent<ColoredObject>();
                    if (colored != null)
                    {
                        colored.gameObject.tag = "Untagged";
                    }
                }
            }
            // Force player out of push state immediately
            _player?.ForceExitPushState();
            if (_keyPrefab != null)
            {
                Vector3 spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position + Vector3.up;
                GameObject newKey = Instantiate(_keyPrefab, spawnPos, Quaternion.identity);
                StartCoroutine(KeySpawnSequenceUtility.PlayKeySpawnSequence(newKey, 2f, 3f, _player)); // Default values, adjust as needed
            }

        }
    }
}