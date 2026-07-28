using Controller;
using General.Enums;
using Manager;
using Unity.Collections;
using UnityEngine;

namespace General.Units
{
    /// <summary>
    /// Shared behavior for melee combat units: walk toward the enemy, and once
    /// in range, strike the nearest enemy on a delay/interval attack cycle.
    /// <see cref="UnitWarrior"/> and <see cref="UnitLancer"/> use this directly;
    /// they exist as distinct types so each can carry its own prefab, stats,
    /// and animator controller in the Unity Editor.
    /// </summary>
    public abstract class MeleeUnit : Unit
    {
        private float _attackDelayTimer;
        private float _attackIntervalTimer;
        private bool _attackComplete;

        public override void Initialize()
        {
            base.Initialize();
            _attackDelayTimer = 0f;
            _attackIntervalTimer = 0f;
            _attackComplete = false;
        }

        private void Update()
        {
            base.OnUpdate();
            OnUpdate();
        }

        public override void OnUpdate()
        {
            if (!Initialized || !Activated)
                return;

            UnitState targetState = GetTargetState();
            if (targetState != CurrentState)
                ProcessStateTransition(targetState);

            switch (CurrentState)
            {
                case UnitState.Idle:
                    break;
                case UnitState.Run:
                    MoveForward();
                    break;
                case UnitState.Attack:
                    DoAttack();
                    break;
                default:
                    Debug.LogError($"[{GetType().Name}] Unhandled state {CurrentState}");
                    break;
            }
        }

        private void DoAttack()
        {
            if (_attackIntervalTimer == 0f)
                AnimationController.TriggerAnimation(UnitAnimationController.AttackTriggerName);

            _attackDelayTimer += Time.deltaTime;
            _attackIntervalTimer += Time.deltaTime;
            if (_attackDelayTimer < BaseUnitStat.AttackDelay)
            {
                AnimationController.TriggerAnimation(UnitAnimationController.AttackEndTriggerName);
                return;
            }

            if (_attackIntervalTimer > BaseUnitStat.AttackInterval)
            {
                _attackDelayTimer = 0f;
                _attackIntervalTimer = 0f;
                _attackComplete = false;
            }

            if (_attackComplete)
                return;

            ContactFilter2D targetFilter = IsEnemyUnit
                ? GameManager.instance.PlayerUnitFilter2D
                : GameManager.instance.EnemyUnitFilter2D;

            using (NativeArray<RaycastHit2D> hits = GetTargetsAhead(targetFilter))
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (IsValidTarget(hit) && hit.collider.TryGetComponent(out Unit unit) && unit.IsActivated())
                    {
                        unit.TakeDamage(BaseUnitStat.Damage);
                    }
                }
            }

            _attackComplete = true;
        }

        private void ProcessStateTransition(UnitState targetState)
        {
            switch (targetState)
            {
                case UnitState.Idle:
                    AnimationController.ResetAllAnimations();
                    break;
                case UnitState.Run:
                    AnimationController.SetBoolAnimation(UnitAnimationController.RunStateBoolName, true);
                    break;
                case UnitState.Attack:
                    AnimationController.SetBoolAnimation(UnitAnimationController.RunStateBoolName, false);
                    break;
                default:
                    Debug.LogError($"[{GetType().Name}] Unhandled state {CurrentState}");
                    break;
            }

            CurrentState = targetState;
        }

        private UnitState GetTargetState()
        {
            if (IsEnemyInAttackRange())
                return UnitState.Attack;

            return UnitState.Run;
        }

        private void MoveForward()
        {
            float targetX = transform.position.x + (BaseUnitStat.Speed * Time.deltaTime * transform.localScale.x);
            transform.position = new Vector3(ClampToLevelBounds(targetX), transform.position.y, transform.position.z);
        }
    }
}