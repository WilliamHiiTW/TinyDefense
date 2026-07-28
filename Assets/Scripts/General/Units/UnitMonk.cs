using Controller;
using General.Enums;
using Unity.Collections;
using UnityEngine;

namespace General.Units
{
    /// <summary>
    /// Support unit that walks the lane and heals the nearest injured allied unit ahead.
    /// If an ally is blocking the path but is already at full health, the monk waits
    /// (Idle) rather than walking through it; it only resumes running once the path
    /// ahead is clear.
    /// </summary>
    public class UnitMonk : Unit
    {
        public ContactFilter2D HealTargetContactFilter2D;

        private float _healDelayTimer;
        private float _healIntervalTimer;
        private bool _healComplete;

        public override void Initialize()
        {
            base.Initialize();
            _healDelayTimer = 0f;
            _healIntervalTimer = 0f;
            _healComplete = false;
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
                case UnitState.Heal:
                    DoHeal();
                    break;
                default:
                    Debug.LogError($"[UnitMonk] Unhandled state {CurrentState}");
                    break;
            }
        }

        private void DoHeal()
        {
            if (_healIntervalTimer == 0f)
                AnimationController.TriggerAnimation(UnitAnimationController.HealTriggerName);

            _healDelayTimer += Time.deltaTime;
            _healIntervalTimer += Time.deltaTime;
            if (_healDelayTimer < BaseUnitStat.AttackDelay)
            {
                AnimationController.TriggerAnimation(UnitAnimationController.HealEndTriggerName);
                return;
            }

            if (_healIntervalTimer > BaseUnitStat.AttackInterval)
            {
                _healDelayTimer = 0f;
                _healIntervalTimer = 0f;
                _healComplete = false;
            }

            if (_healComplete)
                return;

            using (NativeArray<RaycastHit2D> hits = GetTargetsAhead(HealTargetContactFilter2D))
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject == gameObject || hit.collider.gameObject == Spawner)
                        continue;

                    if (hit.collider.TryGetComponent(out Unit unit) && unit.IsActivated() && !unit.IsFullHealth())
                        unit.TakeDamage(-BaseUnitStat.Damage);
                }
            }

            _healComplete = true;
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
                case UnitState.Heal:
                    AnimationController.SetBoolAnimation(UnitAnimationController.RunStateBoolName, false);
                    break;
                default:
                    Debug.LogError($"[UnitMonk] Unhandled state {CurrentState}");
                    break;
            }

            CurrentState = targetState;
        }

        private UnitState GetTargetState()
        {
            if (!TryGetUnitAhead(HealTargetContactFilter2D, out Unit unitAhead))
                return UnitState.Run;

            return unitAhead.IsFullHealth() ? UnitState.Idle : UnitState.Heal;
        }

        /// <summary>Finds the first active ally directly ahead, regardless of its health.</summary>
        private bool TryGetUnitAhead(ContactFilter2D targetFilter, out Unit unitAhead)
        {
            using NativeArray<RaycastHit2D> hits = GetTargetsAhead(targetFilter);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == gameObject || hit.collider.gameObject == Spawner)
                    continue;

                if (hit.collider.TryGetComponent(out Unit unit) && unit.IsActivated() && unit is not UnitPawn)
                {
                    unitAhead = unit;
                    return true;
                }
            }

            unitAhead = null;
            return false;
        }

        private void MoveForward()
        {
            float targetX = transform.position.x + (BaseUnitStat.Speed * Time.deltaTime * transform.localScale.x);
            transform.position = new Vector3(ClampToLevelBounds(targetX), transform.position.y, transform.position.z);
        }
    }
}