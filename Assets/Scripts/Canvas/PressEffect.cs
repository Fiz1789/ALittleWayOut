using UnityEngine;
using KBCore.Refs;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;


namespace Platformer
{
    public class PressEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("References")]
        public RectTransform targetRect;
        public TextMeshProUGUI targetText;
        [Header("Settings")]
        public float hoverScale = 1.1f;
        public float normalScale = 1f;
        public Color normalColor = Color.white;
        public Color clickColor = Color.yellow;
        void OnEnable()
        {
            SetNormal();
        }
        void Reset()
        {
            targetRect = GetComponent<RectTransform>();
            targetText = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Awake()
        {
            if (targetRect == null)
                targetRect = GetComponent<RectTransform>();
            if (targetText == null)
                targetText = GetComponentInChildren<TextMeshProUGUI>();
            SetNormal();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetRect != null)
                targetRect.localScale = Vector3.one * hoverScale;
            if (targetText != null)
                targetText.color = normalColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetNormal();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (targetText != null)
                targetText.color = clickColor;
        }

        void SetNormal()
        {
            if (targetRect != null)
                targetRect.localScale = Vector3.one * normalScale;
            if (targetText != null)
                targetText.color = normalColor;
        }
    }
}
