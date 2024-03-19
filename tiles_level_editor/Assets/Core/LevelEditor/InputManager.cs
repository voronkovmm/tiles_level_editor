using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.LevelEditor
{
    public class InputManager : MonoBehaviour
    {
        public static event Action OnHoldLKM;
        public static event Action OnHoldPKM;
        public static event Action OnPressW;
        public static event Action OnReleaseW;
        public static event Action OnPressQ;
        public static event Action<int> OnPressAlpha;

        public static bool LockInput;

        private Dictionary<KeyCode, Action> getKeyEvents     = new();
        private Dictionary<KeyCode, Action> getKeyDownEvents = new();
        private Dictionary<KeyCode, Action> getKeyUpEvents   = new();

        private void Awake()
        {
            getKeyEvents.Add(KeyCode.Mouse0, () => OnHoldLKM?.Invoke());
            getKeyEvents.Add(KeyCode.Mouse1, () => OnHoldPKM?.Invoke());

            getKeyDownEvents.Add(KeyCode.Alpha1, () => OnPressAlpha?.Invoke(1));
            getKeyDownEvents.Add(KeyCode.Alpha2, () => OnPressAlpha?.Invoke(2));
            getKeyDownEvents.Add(KeyCode.Alpha3, () => OnPressAlpha?.Invoke(3));
            getKeyDownEvents.Add(KeyCode.Alpha4, () => OnPressAlpha?.Invoke(4));
            getKeyDownEvents.Add(KeyCode.Alpha5, () => OnPressAlpha?.Invoke(5));

            getKeyDownEvents.Add(KeyCode.W, () => OnPressW?.Invoke());
            getKeyUpEvents.Add(KeyCode.W, () => OnReleaseW?.Invoke());
            
            getKeyDownEvents.Add(KeyCode.Q, () => OnPressQ?.Invoke());
        }

        private void LateUpdate()
        {
            foreach (var pair in getKeyUpEvents)
            {
                if (Input.GetKeyUp(pair.Key))
                    pair.Value?.Invoke();
            }

            if (LockInput)
                return;

            foreach (var pair in getKeyEvents)
            {
                if(Input.GetKey(pair.Key))
                    pair.Value?.Invoke();
            }

            foreach (var pair in getKeyDownEvents)
            {
                if (Input.GetKeyDown(pair.Key))
                    pair.Value?.Invoke();
            }
        }
    }
}