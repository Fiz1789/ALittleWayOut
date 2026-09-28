using UnityEngine;


namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] LayerMask groundLayers;
        [SerializeField] float groundDistance = 0.1f, radius = 0.5f;

        public bool IsGrounded { get; private set; }

        void Update()
        {
            IsGrounded = Physics.SphereCast(transform.position, radius, Vector3.down, out _, groundDistance, groundLayers);
        }
        void OnDrawGizmos()
        {
            if (IsGrounded)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(transform.position + Vector3.down * groundDistance, radius);


        }
    }
}

