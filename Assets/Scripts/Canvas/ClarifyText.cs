using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace Platformer
{
    public class ClarifyText : MonoBehaviour
    {
        public float totalDuration = 0.3f;
        private TMP_Text tmpText;
        private string fullText;

        void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
            fullText = tmpText.text;
            tmpText.text = fullText;
            tmpText.maxVisibleCharacters = 0;
            tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 0);
        }

        // Call this after your transition is done!
        public void AnimateText()
        {
            int charCount = fullText.Length;
            float revealDuration = totalDuration * 0.8f;
            float fadeDuration = totalDuration;

            DOTween.To(() => tmpText.maxVisibleCharacters,
                       x => tmpText.maxVisibleCharacters = x,
                       charCount, revealDuration)
                   .SetEase(Ease.OutQuad);

            float alpha = 0;
            DOTween.To(() => alpha,
                       x =>
                       {
                           alpha = x;
                           var c = tmpText.color;
                           tmpText.color = new Color(c.r, c.g, c.b, x);
                       },
                       1f, fadeDuration)
                   .SetEase(Ease.OutQuad);
        }
    }
}