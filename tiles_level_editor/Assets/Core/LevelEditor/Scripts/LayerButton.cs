using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.LevelEditor
{
	public class LayerButton : MonoBehaviour
	{
        private Image img;

        private void Awake() => img = GetComponent<Image>();

        public Color Color { set => img.color = value; }
	} 
}