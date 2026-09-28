using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using KBCore.Refs;

namespace Platformer
{
    public class TutorialMovementAppear : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialMovementPanel;
        public void ShowPanel()
        {
            tutorialMovementPanel.SetActive(true);
        }

        public void HidePanel()
        {
            tutorialMovementPanel.SetActive(false);
        }
    }
}