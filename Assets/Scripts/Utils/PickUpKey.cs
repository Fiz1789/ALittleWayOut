using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
    public class PickUpKey : MonoBehaviour
    {

        public static bool isPickedUp = false;
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PickUp();
            }
        }

        void PickUp()
        {
            // Play sound
            // if (pickUpSound != null)
            // {
            //     pickUpSound.Play();
            // }
            isPickedUp = true;
            // Disable key object
            gameObject.SetActive(false);
            // Destroy(this.gameObject);
        }
    }
}