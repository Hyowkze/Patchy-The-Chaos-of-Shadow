using UnityEngine;
using Core.Pooling;

namespace Core.Combat
{
    public class HomingProjectile : PoolableObject
    {
        private float damage;
        private float speed;
        private float maxDistance;
        private Vector2 startPosition;
        private Vector2 direction;
        private LayerMask targetLayer;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Initialize(float damage, float speed, float maxDistance, Vector2 direction, LayerMask targetLayer)
        {
            this.damage = damage;
            this.speed = speed;
            this.maxDistance = maxDistance;
            this.direction = direction;
            this.targetLayer = targetLayer;
            startPosition = transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }

        private void Update()
        {
            Vector2 currentPos = transform.position;
            
            if (Vector2.Distance(startPosition, currentPos) >= maxDistance)
            {
                ReturnToPool();
                return;
            }

            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & targetLayer) != 0)
            {
                if (other.TryGetComponent<IAttackable>(out var attackable))
                {
                    attackable.TakeDamage(damage);
                    ReturnToPool();
                }
            }
        }

        public override void OnSpawnFromPool()
        {
            base.OnSpawnFromPool();
            gameObject.SetActive(true);
        }

        public override void ReturnToPool()
        {
            base.ReturnToPool();
            gameObject.SetActive(false);
        }
    }
}
