using UnityEngine;
using KBCore.Refs;

namespace Platformer
{
    public class RainbowPlatform : ValidatedMonoBehaviour

    {
        [Header("Platform Visuals")]
        [SerializeField, Self] private Renderer platformRenderer;
        [SerializeField] private Color colorOnEnter = Color.white;
        [SerializeField] private Color colorOnExit = Color.black;
        [SerializeField] public RainbowColor expectedColor;
        [SerializeField, Parent] RainbowPuzzleManager rainbowPuzzleManager;
        private bool isCorrect = false;

        void OnTriggerEnter(Collider other)
        {
            // Change material color to white
            if (other.CompareTag("Pushable")){
            if (platformRenderer != null)
                platformRenderer.material.color = colorOnEnter;
            ColoredObject colored = other.GetComponent<ColoredObject>();
            if (colored != null)
            {
                bool correct = colored.color == expectedColor;
                isCorrect = correct;
                rainbowPuzzleManager?.NotifyPlatform(this, correct);
            }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Pushable")){
            // Change material color to black
            if (platformRenderer != null)
                platformRenderer.material.color = colorOnExit;
            ColoredObject colored = other.GetComponent<ColoredObject>();
            if (colored != null)
            {
                isCorrect = false;
                rainbowPuzzleManager?.NotifyPlatform(this, false);
            }
            }
        }

        public bool IsCorrect() => isCorrect;
    }
}
