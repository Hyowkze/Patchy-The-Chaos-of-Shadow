using UnityEngine;

namespace Core.Config
{
    public abstract class ConfigBase : ScriptableObject
    {
        protected virtual void OnValidate()
        {
            ValidateFields();
        }

        protected abstract void ValidateFields();

        protected void ValidateGreaterThanZero(ref float value, string fieldName)
        {
            if (value <= 0)
            {
                Debug.LogWarning($"{fieldName} in {name} must be greater than zero. Resetting to 0.1.");
                value = 0.1f;
            }
        }
    }
}
