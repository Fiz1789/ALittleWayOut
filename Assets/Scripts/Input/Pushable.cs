using KBCore.Refs;
using UnityEngine;

namespace Platformer
{
	public class Pushable : ValidatedMonoBehaviour
	{
		public bool IsInteracted { get; private set; } = false;
		// private Rigidbody rb;

        [SerializeField,Self] private Rigidbody rb;

		void Awake()
		{
			// rb = GetComponent<Rigidbody>();
			if (rb != null)
				rb.isKinematic = true; // Keep stationary until interacted
		}

		// Call this when the player interacts (e.g., left click)
		public void Interact()
		{
			IsInteracted = true;
			if (rb != null)
				rb.isKinematic = false; // Allow movement after interaction
		}

		// Optional: Call this to reset interaction and make stationary again
		public void ResetInteraction()
		{
			IsInteracted = false;
			if (rb != null)
				rb.isKinematic = true;
		}
	}
}
