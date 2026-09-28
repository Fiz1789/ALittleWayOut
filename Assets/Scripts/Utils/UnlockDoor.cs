// UnlockDoor.cs
using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
    public class UnlockDoor : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Animation doorAnimator;
        // private static readonly int IsOpenHash = Animator.StringToHash("Open");
        // private static readonly int IsCloseHash = Animator.StringToHash("Close");
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && PickUpKey.isPickedUp)
            {
                OpenDoor();
                PickUpKey.isPickedUp = false; // Reset for next key
            }
        }
        public void OpenDoor()
        {
            if (doorAnimator != null)
            {
                doorAnimator.Play("Open");

            }
        }

        public void CloseDoor()
        {
            if (doorAnimator != null)
            {
                doorAnimator.Play("Close");
            }
        }
    }
}