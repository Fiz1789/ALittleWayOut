using UnityEngine;
using DG.Tweening;

namespace Platformer
{
    public class PlatformMover : MonoBehaviour
    {
        [SerializeField] Vector3 moveTo = Vector3.zero;
        [SerializeField] float moveDuration = 1f;
        [SerializeField] Ease ease = Ease.InOutQuad;

        Vector3 startPosition;

        void Start()
        {
            startPosition = transform.position;
            MovePlatform();
        }

        void MovePlatform()
        {
            transform.DOMove(startPosition+moveTo, moveDuration)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
