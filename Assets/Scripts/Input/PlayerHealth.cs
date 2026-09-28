using UnityEngine;
using KBCore.Refs;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth = 10;
        public int currentHealth;

        [Header("Death Screen")]
        [SerializeField, Anywhere] private GameObject deathScreenCanvas;
        [SerializeField, Self] private PlayerController playerController; 

        [SerializeField] private SceneTransition sceneTransition; 
  
        [SerializeField] private float deathScreenDelay = 2f; 

        void Start()
        {
            currentHealth = maxHealth;
        }
        public void PlayerDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                PlayerDie();
            }
        }

        void PlayerDie()
        {
            Debug.Log("Player has died.");
            if (sceneTransition != null && deathScreenCanvas != null)
                StartCoroutine(sceneTransition.FadeOutDeath(1f));
            if (playerController != null)
                playerController.MovementEnabled = false;
            if (deathScreenDelay > 0f)
                Invoke(nameof(ReloadScene), deathScreenDelay);
        }

        void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
