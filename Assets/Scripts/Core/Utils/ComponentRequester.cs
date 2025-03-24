using UnityEngine;

namespace Core.Utils
{
    public abstract class ComponentRequester : MonoBehaviour
    {
        protected virtual void Awake()
        {
            ValidateComponents();
        }

        protected virtual void Start()
        {
            SubscribeToEvents();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        protected virtual void ValidateComponents()
        {
            // Default implementation: does nothing
        }

        protected virtual void SubscribeToEvents() { }
        protected virtual void UnsubscribeFromEvents() { }

        protected T RequestComponent<T>() where T : Component
        {
            T component = GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Missing required component {typeof(T)} on {gameObject.name}");
                enabled = false;
            }
            return component;
        }
    }
}
