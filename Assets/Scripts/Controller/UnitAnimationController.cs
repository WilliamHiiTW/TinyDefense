using UnityEngine;

namespace Controller
{
    /// <summary>
    /// Thin wrapper around a Unity <see cref="Animator"/> that centralizes the
    /// animation parameter names used across all unit types, so callers never
    /// have to reference raw strings directly.
    /// </summary>
    public class UnitAnimationController
    {
        public const string AttackTriggerName = "attackStart";
        public const string AttackEndTriggerName = "attackEnd";
        public const string HealTriggerName = "healStart";
        public const string HealEndTriggerName = "healEnd";
        public const string RunStateBoolName = "isRunning";
        public const string ResourceCollectedBoolName = "resourceCollected";
        public const string IsCollectingBoolName = "isCollecting";
        public const string GoldCollectorBoolName = "GoldCollector";

        public Animator UnitAnimator { get; private set; }
        private bool _initialized;

        public void Initialize(Animator unitAnimator)
        {
            if (unitAnimator == null)
            {
                Debug.LogError("[UnitAnimationController] Animator is not assigned");
                return;
            }

            UnitAnimator = unitAnimator;
            _initialized = true;
        }

        public void TriggerAnimation(string id)
        {
            if (!_initialized)
            {
                Debug.LogError("[UnitAnimationController] has not been initialized");
                return;
            }

            UnitAnimator.SetTrigger(id);
        }

        public void SetBoolAnimation(string id, bool value)
        {
            if (!_initialized)
            {
                Debug.LogError("[UnitAnimationController] has not been initialized");
                return;
            }

            UnitAnimator.SetBool(id, value);
        }

        /// <summary>Resets the animator back to its idle/run-stopped state.</summary>
        public void ResetAllAnimations()
        {
            if (!_initialized)
            {
                Debug.LogError("[UnitAnimationController] has not been initialized");
                return;
            }

            UnitAnimator.SetBool(RunStateBoolName, false);
        }
    }
}
