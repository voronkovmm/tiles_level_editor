using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace Voron
{
    public static class Voron
    {
        public enum EnumAnchor
        {
            CENTER,
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT,
        }

        public static Vector2 TransformWorldPosToAnchore(Camera camera, Vector2 worldPos, EnumAnchor anchorType, RectTransform canvasTransform)
        {
            Vector2 anchore = GetAnchore(anchorType);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, camera.WorldToScreenPoint(worldPos), camera, out Vector2 localPos);
            Vector2 position = new Vector2(localPos.x - canvasTransform.rect.width / 2 * anchore.x, localPos.y - canvasTransform.rect.height / 2 * anchore.y);

            return position;
        }
        public static Vector2 TransformWorldPosToAnchore(Camera camera, Vector2 worldPos, Vector2 anchorePosition, RectTransform canvasTransform)
        {
            Vector2 anchore = anchorePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, camera.WorldToScreenPoint(worldPos), camera, out Vector2 localPos);
            Vector2 position = new Vector2(localPos.x - canvasTransform.rect.width / 2 * anchore.x, localPos.y - canvasTransform.rect.height / 2 * anchore.y);
            return position;
        }

        public static Quaternion RotateToDirection(Vector2 rotateDir) => Quaternion.AngleAxis(Mathf.Atan2(rotateDir.y, rotateDir.x) * Mathf.Rad2Deg, Vector3.forward);

        public static Vector2 TransformToAnchor(Vector2 oldPos, Vector2 oldAnchore, RectTransform rect, EnumAnchor anchorType)
        {
            Vector2 newAchore = GetAnchore(anchorType);

            float width = rect.rect.width;
            float height = rect.rect.height;

            float xCoordinateFromZero = oldAnchore.x * width + oldPos.x;
            float yCoordinateFromZero = oldAnchore.y * height + oldPos.y;

            return new Vector2
            {
                x = xCoordinateFromZero - width * newAchore.x,
                y = yCoordinateFromZero - height * newAchore.y
            };
        }

        public static Vector2 GetAnchore(EnumAnchor anchoreType)
        {
            return anchoreType switch
            {
                EnumAnchor.CENTER => new Vector2(.5f, .5f),
                EnumAnchor.TOP_LEFT => new Vector2(0, 1),
                EnumAnchor.TOP_RIGHT => new Vector2(1, 1),
                EnumAnchor.BOTTOM_LEFT => new Vector2(0, 0),
                EnumAnchor.BOTTOM_RIGHT => new Vector2(1, 0),
                _ => Vector3.zero
            };
        }

        public static IEnumerator ShowTextPopup(Vector3 pos, string text, float time, float size, Color color)
        {
            GameObject gameObject = new GameObject();
            TMP_Text tmp = gameObject.AddComponent<TMP_Text>();
            tmp.text      = text;
            tmp.color     = color;
            tmp.fontSize  = size;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;

            gameObject.transform.position = pos;

            yield return new WaitForSeconds(time);

            UnityEngine.Object.Destroy(gameObject);
        }

        public static void TakeScreenshot(string path, Camera camera, int width, int height, int quality)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            camera.targetTexture = renderTexture;

            Texture2D screenshot = new Texture2D(camera.pixelWidth, camera.pixelHeight, TextureFormat.RGB24, false);

            camera.Render();

            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(new Rect(0, 0, camera.pixelWidth, camera.pixelHeight), 0, 0);
            screenshot.Apply();

            // Задаем качество и сохраняем в формате JPG
            byte[] bytes = screenshot.EncodeToJPG(quality);
            File.WriteAllBytes(path, bytes);

            RenderTexture.active = null;
            camera.targetTexture = null;
            UnityEngine.Object.Destroy(renderTexture);
        }

        public static byte[] TakeScreenshot(Camera camera, int width, int height, int quality)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            camera.targetTexture = renderTexture;

            Texture2D screenshot = new Texture2D(camera.pixelWidth, camera.pixelHeight, TextureFormat.RGB24, false);

            camera.Render();

            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(new Rect(0, 0, camera.pixelWidth, camera.pixelHeight), 0, 0);
            screenshot.Apply();

            byte[] bytes = screenshot.EncodeToJPG(quality);

            RenderTexture.active = null;
            camera.targetTexture = null;
            UnityEngine.Object.Destroy(renderTexture);

            return bytes;
        }
    }

    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> List;

        public int Count => List.Count;

        public T this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }

        public ListWrapper(int capacity) => List = new(capacity);
    }
}
