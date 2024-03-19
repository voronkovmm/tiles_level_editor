#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.LevelEditor
{
    public class EditorPopupLevelSelect : EditorWindow
    {
        private static Action onCloseAction;
        private static Action<LevelDataSO> onLoadAction;

        private int selectedIndex = 0;
        private static List<LevelDataSO> levels;
        private static string[] names;

        public static void ShowWindow(List<LevelDataSO> list, Action<LevelDataSO> onLoad, Action onClose)
        {
            levels = list;
            onLoadAction = onLoad;
            onCloseAction = onClose;
            names = levels.Select(x => x.Name).ToArray();
            GetWindow<EditorPopupLevelSelect>("Select Level");
        }

        private void OnGUI()
        {
            selectedIndex = EditorGUILayout.Popup("Select Level", selectedIndex, names);

            if (GUILayout.Button("Load"))
            {
                if (Application.isPlaying)
                    onLoadAction?.Invoke(levels[selectedIndex]);

                Close();
            }
        }

        private void OnDestroy() => onCloseAction?.Invoke();
    }
}
#endif