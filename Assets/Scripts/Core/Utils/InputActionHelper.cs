using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Utils
{
    public static class InputActionHelper
    {
        public static InputAction FindAndRegisterAction(InputActionMap actionMap, string actionName, Action<InputAction.CallbackContext> callback)
        {
            InputAction action = actionMap.FindAction(actionName);
            if (action == null)
            {
                Debug.LogError($"Action '{actionName}' not found in action map '{actionMap.name}'!");
                return null;
            }
            action.performed += callback;
            return action;
        }

        public static void UnregisterAction(InputAction action, Action<InputAction.CallbackContext> callback)
        {
            if (action != null)
            {
                action.performed -= callback;
            }
        }
    }
}
