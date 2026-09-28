using UnityEngine;

namespace Platformer
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        Transform platform; 
        
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                ContactPoint contact = other.GetContact(0);
                if(contact.normal.y < 0.5f)
                    return; // Ignore side collisions
                platform = other.transform;
                transform.SetParent(platform);
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
                platform = null;
            }
        }
    }
}
