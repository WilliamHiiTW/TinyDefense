using Controller;
using General.Enums;
using Manager;
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
        public bool IsEnemyUnit { get; set; }

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

            return IsTargetInAttackRange(enemyFilter);
        }

        /// <summary>Checks whether any collider matching <paramref name="targetFilter"/> is within attack range.</summary>
        protected bool IsTargetInAttackRange(ContactFilter2D targetFilter)
        {
            ColliderArray2D hits = Physics2D.OverlapCircle(transform.position, BaseUnitStat.AttackRange, targetFilter);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject != gameObject && hit.gameObject != Spawner && hit.gameObject.layer != gameObject.layer)
                {
                    hits.Dispose();
                    return true;
                }
            }

            hits.Dispose();
            return false;
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
            Gizmos.DrawWireSphere(transform.position, BaseUnitStat.AttackRange);
        }
    }
}
