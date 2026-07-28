using Controller;
using General.Battle;
using General.Enums;
using UnityEngine;

namespace General.Units
{
    /// <summary>Ranged unit that fires pooled <see cref="Projectile"/>s at enemies in range.</summary>
    public class UnitArcher : Unit
    {
        public ProjectileController ProjectileController;

        private bool _arrowShot;
        private float _attackDelayTimer;
        private float _attackIntervalTimer;

        public override void Initialize()
        {
            base.Initialize();
            ProjectileController.Initialize();
            _attackDelayTimer = 0f;
            _attackIntervalTimer = 0f;
            _arrowShot = false;
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
                    ShootArrow();
                    break;
                default:
                    Debug.LogError($"[UnitArcher] Unhandled state {CurrentState}");
                    break;
            }
        }

        private void ShootArrow()
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
                _arrowShot = false;
            }

            if (_arrowShot)
                return;

            Projectile spawned = ProjectileController.Spawn();
            Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
            Vector2 spawnPosition = new Vector2(transform.position.x + (1f * transform.localScale.x), transform.position.y);
            spawned.OnStart(spawnPosition, direction, gameObject.layer);
            _arrowShot = true;
        }

        private UnitState GetTargetState()
        {
            if (IsEnemyInAttackRange())
                return UnitState.Attack;

            return UnitState.Run;
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
                    Debug.LogError($"[UnitArcher] Unhandled state {CurrentState}");
                    break;
            }

            CurrentState = targetState;
        }

        private void MoveForward()
        {
            float targetX = transform.position.x + (BaseUnitStat.Speed * Time.deltaTime * transform.localScale.x);
            transform.position = new Vector3(ClampToLevelBounds(targetX), transform.position.y, transform.position.z);
        }
    }
}