using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Core.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class Gost : MonoBehaviour
    {
        [SerializeField] private Color colorHightlightCell;
        private Image image;
        private RectTransform rect;

        public bool Enable { set => gameObject.SetActive(value); }

        public void Initialize(Vector3 size, RectTransform parent)
        {
            image = GetComponent<Image>();
            rect = transform as RectTransform;
            image.color = colorHightlightCell;
            rect.sizeDelta = size;
            rect.SetParent(parent, false);
        }
    }
}