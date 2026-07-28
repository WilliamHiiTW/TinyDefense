using Controller;
using General.Enums;
using Manager;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace General
{
    /// <summary>
    /// Base class for all controllable and enemy units. Handles shared state
    /// (health, activation, animation) and range-query helpers used by
    /// subclasses to implement their specific attack/heal/interact behavior.
    /// </summary>
    public abstract class Unit : MonoBehaviour
    {
        public BaseUnitStat BaseUnitStat;
        public GameObject Spawner;
        public Animator UnitAnimator;
        public UnitState CurrentState;
        public UnitAnimationController AnimationController;
        public Slider HealthBar;

        protected bool Initialized;
        protected bool Activated;
        protected float _currentHealth;

        /// <summary>True if this unit belongs to the enemy side.</summary>
        public bool IsEnemyUnit;

        private void Start()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            if (BaseUnitStat == null)
                Debug.LogError("[Unit] Base Unit Stat is NOT assigned for this unit.");

            if (UnitAnimator == null)
                Debug.LogError("[Unit] UnitAnimator is NOT assigned for this unit.");

            AnimationController = new UnitAnimationController();
            AnimationController.Initialize(UnitAnimator);
            CurrentState = UnitState.None;
            _currentHealth = BaseUnitStat.Health;
            Spawner = IsEnemyUnit
                ? GameManager.instance.EnemyTower.gameObject
                : GameManager.instance.PlayerTower.gameObject;
            Initialized = true;
        }

        public virtual void OnUpdate()
        {
            if (GameManager.instance.GameOver)
                DeactivateSelf();

            if (HealthBar != null)
                HealthBar.value = Mathf.Clamp01(_currentHealth / BaseUnitStat.Health);
        }

        /// <summary>Checks whether an opposing unit is within attack range, excluding self and the spawner.</summary>
        protected bool IsEnemyInAttackRange()
        {
            ContactFilter2D enemyFilter = IsEnemyUnit
                ? GameManager.instance.PlayerUnitFilter2D
                : GameManager.instance.EnemyUnitFilter2D;

            using NativeArray<RaycastHit2D> hits = GetTargetsAhead(enemyFilter);
            foreach (RaycastHit2D hit in hits)
            {
                if (IsValidTarget(hit) && hit.collider.gameObject.TryGetComponent(out Unit unit) && unit.IsActivated())
                    return true;
            }

            return false;
        }

        /// <summary>Checks whether any collider matching <paramref name="targetFilter"/> is directly ahead within attack range.</summary>
        protected bool IsTargetInAttackRange(ContactFilter2D targetFilter)
        {
            using NativeArray<RaycastHit2D> hits = GetTargetsAhead(targetFilter);
            foreach (RaycastHit2D hit in hits)
            {
                if (IsValidTarget(hit))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Casts a straight line in this unit's facing direction, out to its attack range,
        /// and returns every collider matching <paramref name="targetFilter"/> along that line.
        /// A straight-line query (rather than a radius) matters here since lanes/paths run
        /// parallel to each other — a radius check would "see" units in a neighboring lane,
        /// while a same-height raycast only ever hits targets in this unit's own lane.
        /// The caller is responsible for disposing the returned NativeArray (a `using`
        /// statement/declaration handles this automatically).
        /// </summary>
        protected NativeArray<RaycastHit2D> GetTargetsAhead(ContactFilter2D targetFilter)
        {
            Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
            Vector2 origin = transform.position;
            return Physics2D.Raycast(origin, direction, targetFilter, BaseUnitStat.AttackRange, Allocator.Temp);
        }

        /// <summary>Excludes self and this unit's own spawner/tower from a raycast hit.</summary>
        protected bool IsValidTarget(RaycastHit2D hit)
        {
            return hit.collider.gameObject != gameObject
                && hit.collider.gameObject != Spawner
                && hit.collider.gameObject.layer != gameObject.layer;
        }

        /// <summary>
        /// Clamps an X position to the level's configured playable bounds
        /// (<see cref="GameManager.LevelMinX"/> / <see cref="GameManager.LevelMaxX"/>),
        /// so units can never walk off the edge of the map when nothing else stops them.
        /// </summary>
        protected float ClampToLevelBounds(float x)
        {
            return Mathf.Clamp(x, GameManager.instance.LevelMinX, GameManager.instance.LevelMaxX);
        }

        public float TakeDamage(float damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
                DeactivateSelf();

            return _currentHealth;
        }

        public virtual void Activate()
        {
            Activated = true;
        }

        public bool IsFullHealth()
        {
            return _currentHealth >= BaseUnitStat.Health;
        }

        public bool IsHealthEmpty()
        {
            return _currentHealth <= 0;
        }

        public bool IsActivated()
        {
            return Activated;
        }

        private void DeactivateSelf()
        {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmosSelected()
        {
            if (BaseUnitStat == null)
                return;

            Gizmos.color = Color.red;
            Vector3 direction = new Vector3(Mathf.Sign(transform.localScale.x), 0f, 0f);
            Gizmos.DrawLine(transform.position, transform.position + direction * BaseUnitStat.AttackRange);
        }
    }
}