using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
    public class BrokenWall : MonoBehaviour
    {
        [Header("Wall References")]
        [SerializeField, Anywhere] GameObject notBrokenWall;
        [SerializeField, Anywhere] GameObject brokenWall;

        [Header("Force Settings")]
        [SerializeField] float breakForce = 8f;

        bool hasBroken = false;

        void OnTriggerEnter(Collider other)
        {
            if (hasBroken) return;

            if (other.CompareTag("Player"))
            {
                BreakWall();
            }
        }

        void BreakWall()
        {
            hasBroken = true;

            // Hide intact wall
            notBrokenWall.SetActive(false);

            // Show broken wall
            brokenWall.SetActive(true);

            // Apply force
            Rigidbody[] rigidbodies = brokenWall.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = false;

                // Push forward (Z axis)
                rb.AddForce(brokenWall.transform.forward * breakForce, ForceMode.Impulse);
            }
            Destroy(brokenWall, 5f);
        }
    }
}