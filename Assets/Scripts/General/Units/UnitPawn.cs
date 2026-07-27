using Controller;
using General.Enums;
using Manager;
using UnityEngine;

namespace General.Units
{
    /// <summary>
    /// Worker unit that walks to a resource/interact target, gathers it, then
    /// carries it back to the spawner (tower) to convert it into gold.
    /// </summary>
    public class UnitPawn : Unit
    {
        public ContactFilter2D InteractTargetContactFilter2D;
        public ContactFilter2D SpawnerContactFilter2D;

        private float _direction;
        private bool _isCarryingResource;
        private float _interactDuration;

        public override void Initialize()
        {
            base.Initialize();
            _interactDuration = 0f;
            AnimationController.SetBoolAnimation(UnitAnimationController.GoldCollectorBoolName, true);
        }

        public override void Activate()
        {
            base.Activate();
            _direction = transform.localScale.x;
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
                case UnitState.Interact:
                    DoInteract();
                    break;
                default:
                    Debug.LogError($"[UnitPawn] Unhandled state {CurrentState}");
                    break;
            }
        }

        private void DoInteract()
        {
            AnimationController.SetBoolAnimation(UnitAnimationController.IsCollectingBoolName, true);
            if (_interactDuration > BaseUnitStat.AttackDelay)
                _isCarryingResource = true;

            _interactDuration += Time.deltaTime;
        }

        private void ProcessStateTransition(UnitState targetState)
        {
            switch (targetState)
            {
                case UnitState.Idle:
                    AnimationController.ResetAllAnimations();
                    break;
                case UnitState.Run:
                    if (!_isCarryingResource)
                    {
                        AnimationController.SetBoolAnimation(UnitAnimationController.RunStateBoolName, true);
                    }
                    else
                    {
                        FlipFacing();
                        AnimationController.SetBoolAnimation(UnitAnimationController.ResourceCollectedBoolName, true);
                    }
                    break;
                case UnitState.Interact:
                    AnimationController.SetBoolAnimation(UnitAnimationController.RunStateBoolName, false);
                    break;
                default:
                    Debug.LogError($"[UnitPawn] Unhandled state {CurrentState}");
                    break;
            }

            CurrentState = targetState;
        }

        private bool HasReachedSpawner()
        {
            ColliderArray2D hits = Physics2D.OverlapCircle(transform.position, BaseUnitStat.AttackRange, SpawnerContactFilter2D);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == Spawner)
                {
                    hits.Dispose();
                    return true;
                }
            }

            hits.Dispose();
            return false;
        }

        private UnitState GetTargetState()
        {
            if (_isCarryingResource)
            {
                if (HasReachedSpawner())
                {
                    _isCarryingResource = false;
                    _interactDuration = 0f;
                    GameManager.instance.PlayerGold += 1;
                    AnimationController.SetBoolAnimation(UnitAnimationController.ResourceCollectedBoolName, false);
                    FlipFacing();
                }

                return UnitState.Run;
            }

            if (IsTargetInAttackRange(InteractTargetContactFilter2D))
                return UnitState.Interact;

            return UnitState.Run;
        }

        private void FlipFacing()
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        private void MoveForward()
        {
            float direction = _isCarryingResource ? -_direction : _direction;
            transform.position = new Vector3(
                transform.position.x + (BaseUnitStat.Speed * Time.deltaTime * direction),
                transform.position.y,
                transform.position.z);
        }
    }
}
