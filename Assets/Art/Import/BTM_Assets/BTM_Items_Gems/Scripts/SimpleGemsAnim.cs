using UnityEngine;
using System.Collections;

namespace Platformer
{
    public class SimpleGemsAnim : MonoBehaviour
    {
        public bool isRotating = false;
        public bool rotateX = false;
        public bool rotateY = false;
        public bool rotateZ = false;
        public float rotationSpeed = 90f; // Degrees per second

        public bool isFloating = false;
        public bool useEasingForFloating = false; // Separate toggle for floating ease
        public float floatHeight = 1f; // Max height displacement
        public float floatSpeed = 1f;
        public enum FloatDirection { X_Up, X_Down, Y_Up, Y_Down, Z_Up,Z_Down} // Direction of float movement
        public FloatDirection floatDirection = FloatDirection.Y_Up; // Default to Y up/down
        private Vector3 initialPosition;
        private float floatTimer;

        private Vector3 initialScale;
        public Vector3 startScale;
        public Vector3 endScale;

        public bool isScaling = false;
        public bool useEasingForScaling = false; // Separate toggle for scaling ease
        public float scaleLerpSpeed = 1f; // Speed of scaling transition
        private float scaleTimer;

        void Start()
        {
            initialScale = transform.localScale;
            initialPosition = transform.position;

            // Adjust start and end scale based on initial scale
            startScale = initialScale;
            endScale = initialScale * (endScale.magnitude / startScale.magnitude);
        }

        void Update()
        {
            if (isRotating)
            {
                Vector3 rotationVector = new Vector3(
                    rotateX ? 1 : 0,
                    rotateY ? 1 : 0,
                    rotateZ ? 1 : 0
                );
                transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
            }

            if (isFloating)
            {
                floatTimer += Time.deltaTime * floatSpeed;
                float t = Mathf.PingPong(floatTimer, 1f);
                if (useEasingForFloating) t = EaseInOutQuad(t);

                Vector3 offset = Vector3.zero;
                switch (floatDirection)
                {
                    case FloatDirection.X_Up:
                        offset = new Vector3(t * floatHeight, 0, 0);
                        break;
                    case FloatDirection.X_Down:
                        offset = new Vector3(-t * floatHeight, 0, 0);
                        break;
                    case FloatDirection.Y_Up:
                        offset = new Vector3(0, t * floatHeight, 0);
                        break;
                    case FloatDirection.Y_Down:
                        offset = new Vector3(0, -t * floatHeight, 0);
                        break;
                    case FloatDirection.Z_Up:
                        offset = new Vector3(0, 0, t * floatHeight);
                        break;
                    case FloatDirection.Z_Down:
                        offset = new Vector3(0, 0, -t * floatHeight);
                        break;
                }

                transform.position = initialPosition + offset;
            }

            if (isScaling)
            {
                scaleTimer += Time.deltaTime * scaleLerpSpeed;
                float t = Mathf.PingPong(scaleTimer, 1f); // Oscillates between 0 and 1

                if (useEasingForScaling)
                {
                    t = EaseInOutQuad(t);
                }

                transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }
        }

        float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }
    }
}

