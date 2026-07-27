using Controller;
using General.Enums;
using Manager;
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

            ColliderArray2D hits = Physics2D.OverlapCircle(transform.position, BaseUnitStat.AttackRange, GameManager.instance.EnemyUnitFilter2D);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject != gameObject && hit.gameObject != Spawner && hit.gameObject.layer != gameObject.layer)
                {
                    if (hit.TryGetComponent(out Unit unit))
                        unit.TakeDamage(BaseUnitStat.Damage);
                }
            }

            hits.Dispose();
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
            transform.position = new Vector3(
                transform.position.x + (BaseUnitStat.Speed * Time.deltaTime * transform.localScale.x),
                transform.position.y,
                transform.position.z);
        }
    }
}
