using Controller;
using General.Battle;
using Manager;
using UnityEngine;

namespace General.Units
{
    /// <summary>
    /// The player's or enemy's base structure. Loses when its health reaches zero.
    /// Also defends itself by firing at the nearest opposing unit within range —
    /// this covers any lane that's been fully cleared of units, since a stationary
    /// tower needs to see in every direction, unlike mobile units which only ever
    /// need to check straight ahead in their own lane.
    /// </summary>
    public class UnitTower : Unit
    {
        public ProjectileController ProjectileController;

        private float _attackDelayTimer;
        private float _attackIntervalTimer;
        private bool _attackComplete;

        public override void Initialize()
        {
            _currentHealth = BaseUnitStat.Health;
            ProjectileController.Initialize();
            _attackDelayTimer = 0f;
            _attackIntervalTimer = 0f;
            _attackComplete = false;
            Activated = true;
        }

        private void Update()
        {
            base.OnUpdate();
            DoAttack();
        }

        private void DoAttack()
        {
            Collider2D target = FindNearestEnemy();
            if (target == null)
            {
                // Nothing in range right now — reset the cycle so the tower fires
                // immediately once a target does come into range, instead of
                // resuming a stale countdown from before the lane emptied out.
                _attackDelayTimer = 0f;
                _attackIntervalTimer = 0f;
                _attackComplete = false;
                return;
            }

            _attackDelayTimer += Time.deltaTime;
            _attackIntervalTimer += Time.deltaTime;
            if (_attackDelayTimer < BaseUnitStat.AttackDelay)
                return;

            if (_attackIntervalTimer > BaseUnitStat.AttackInterval)
            {
                _attackDelayTimer = 0f;
                _attackIntervalTimer = 0f;
                _attackComplete = false;
            }

            if (_attackComplete)
                return;

            FireAt(target);
            _attackComplete = true;
        }

        /// <summary>
        /// Finds the closest opposing unit within range using an omnidirectional
        /// circle check — appropriate here since, unlike mobile lane units, the
        /// tower needs to detect targets across all lanes, not just straight ahead.
        /// </summary>
        private Collider2D FindNearestEnemy()
        {
            ContactFilter2D enemyFilter = IsEnemyUnit
                ? GameManager.instance.PlayerUnitFilter2D
                : GameManager.instance.EnemyUnitFilter2D;

            ColliderArray2D hits = Physics2D.OverlapCircle(transform.position, BaseUnitStat.AttackRange, enemyFilter);

            Collider2D nearest = null;
            float nearestDistance = float.MaxValue;
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;

                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = hit;
                }
            }

            hits.Dispose();
            return nearest;
        }

        private void FireAt(Collider2D target)
        {
            Projectile spawned = ProjectileController.Spawn();
            Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            spawned.OnStart(transform.position, direction, gameObject.layer);
        }

        /// <summary>
        /// Overrides the base line gizmo (correct for raycast-based lane units) with a
        /// circle, matching this tower's actual omnidirectional OverlapCircle detection.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (BaseUnitStat == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, BaseUnitStat.AttackRange);
        }
    }
}