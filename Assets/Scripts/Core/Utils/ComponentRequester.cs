using UnityEngine;

namespace Core.Utils
{
    public abstract class ComponentRequester : MonoBehaviour
    {
        protected virtual void Awake()
        {
            ValidateComponents();
        }

        protected abstract void ValidateComponents();

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

        protected virtual void OnEnable()
        {
            SubscribeToEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        protected virtual void SubscribeToEvents() { }
        protected virtual void UnsubscribeFromEvents() { }
    }
}
