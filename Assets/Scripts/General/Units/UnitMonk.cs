using Controller;
using General.Enums;
using UnityEngine;

namespace General.Units
{
    /// <summary>Support unit that walks the lane and heals the nearest injured allied unit in range.</summary>
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

            ColliderArray2D hits = Physics2D.OverlapCircle(transform.position, BaseUnitStat.AttackRange, HealTargetContactFilter2D);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject || hit.gameObject == Spawner)
                    continue;

                if (hit.TryGetComponent(out Unit unit) && unit.IsActivated() && !unit.IsFullHealth())
                    unit.TakeDamage(-BaseUnitStat.Damage);
            }

            hits.Dispose();
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
            return HasHealTargetAhead(HealTargetContactFilter2D) ? UnitState.Heal : UnitState.Run;
        }

        private bool HasHealTargetAhead(ContactFilter2D targetFilter)
        {
            ColliderArray2D hits = Physics2D.OverlapCircle(transform.position, BaseUnitStat.AttackRange, targetFilter);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject || hit.gameObject == Spawner)
                    continue;

                if (hit.TryGetComponent(out Unit unit) && unit.IsActivated() && !unit.IsFullHealth())
                {
                    hits.Dispose();
                    return true;
                }
            }

            hits.Dispose();
            return false;
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
