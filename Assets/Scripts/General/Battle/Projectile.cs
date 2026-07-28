using System;
using Manager;
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

        private Vector2 _direction;
        private float _remainingLifeTime;
        private int _shooterLayer;

        /// <summary>
        /// Positions and (re)activates the projectile at the start of its flight, aiming
        /// it toward <paramref name="direction"/>. The sprite is rotated to visually face
        /// that direction, so a single shared prefab (art authored facing right at 0°)
        /// works for any shooter/target angle, not just left/right. <paramref name="shooterLayer"/>
        /// is remembered so the projectile can pass harmlessly through its own shooter and
        /// any of the shooter's allies, rather than only checking tag/proximity at spawn.
        /// </summary>
        public void OnStart(Vector2 position, Vector2 direction, int shooterLayer)
        {
            transform.position = position;
            _direction = direction.normalized;
            _remainingLifeTime = LifeTime;
            _shooterLayer = shooterLayer;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void Update()
        {
            if(GameManager.instance.GameOver) Despawn(this);
            _remainingLifeTime -= Time.deltaTime;
            if (_remainingLifeTime <= 0f)
            {
                Despawn(this);
                return;
            }

            transform.position += (Vector3)(_direction * Speed * Time.deltaTime);
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

            // Pass through the shooter itself and any of its allies (same layer) —
            // this is what actually prevents self-hits and friendly fire, rather than
            // relying on spawn position/offset alone.
            if (target.layer == _shooterLayer)
                return;

            
            if (!target.TryGetComponent(out Unit unit))
            {
                return;
            }

            if (unit.IsActivated())
            {
                unit.TakeDamage(Damage);
                Despawn(this);
            }

        }
    }
}