// EnemyExperienceConfig.cs (Nuevo Scriptable Object)
using UnityEngine;
using Core.Config;

namespace Core.Enemy
{
    [CreateAssetMenu(fileName = "EnemyExperienceConfig", menuName = "Config/EnemyExperienceConfig")]
    public class EnemyExperienceConfig : ConfigBase
    {
        [SerializeField] private float experienceReward = 100f;

        public float ExperienceReward => experienceReward;

        protected override void ValidateFields()
        {
            ValidateGreaterThanZero(ref experienceReward, nameof(experienceReward));
        }
    }
}
