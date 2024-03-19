using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Tile : MonoBehaviour
{
    private Image img;
    private RectTransform rect;

    public bool Enable { set => gameObject.SetActive(value); }
    public Color Color { set => img.color = value; }
    public Vector2 AnchoredPosition => rect.anchoredPosition;

    private void Awake()
    {
        rect = transform as RectTransform;
        img = GetComponent<Image>();
    }

    public void SetSize(Vector2 size) => rect.sizeDelta = size;
    public void SetPosition(Vector2 anchorPos) => rect.anchoredPosition = anchorPos;
}