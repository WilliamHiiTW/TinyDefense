using System;
using UnityEngine;

namespace General.Battle
{
    /// <summary>
    /// A pooled projectile that travels in a straight line and applies damage
    /// to whatever "Damageable" collider it hits. Lifetime-expired or spent
    /// projectiles are returned to the pool via <see cref="Despawn"/>.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        public float Damage;
        public float Speed;
        public float LifeTime;

        /// <summary>Raised when this projectile should be returned to its object pool.</summary>
        public Action<Projectile> Despawn;

        private float _direction;
        private float _remainingLifeTime;

        /// <summary>Positions and (re)activates the projectile at the start of its flight.</summary>
        public void OnStart(Vector2 position, float direction)
        {
            transform.position = position;
            _direction = direction;
            _remainingLifeTime = LifeTime;
        }

        private void Update()
        {
            _remainingLifeTime -= Time.deltaTime;
            if (_remainingLifeTime <= 0f)
            {
                Despawn(this);
                return;
            }

            transform.position = new Vector2(
                transform.position.x + Speed * Time.deltaTime * _direction,
                transform.position.y);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            TryApplyDamageAndDespawn(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryApplyDamageAndDespawn(other.gameObject);
        }

        private void TryApplyDamageAndDespawn(GameObject target)
        {
            if (!target.CompareTag("Damageable"))
                return;

            if (target.TryGetComponent(out Unit unit))
                unit.TakeDamage(Damage);

            Despawn(this);
        }
    }
}
