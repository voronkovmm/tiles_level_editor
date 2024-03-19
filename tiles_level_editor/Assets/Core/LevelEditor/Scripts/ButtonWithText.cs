using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWithText : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private TMP_Text tmp;

    public Color Color { set => img.color = value; }
    public string Text { set => tmp.text = value; }
}
