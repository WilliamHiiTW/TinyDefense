using UnityEngine;

namespace General
{
    /// <summary>
    /// Designer-configurable stat block for a unit type. Create instances via
    /// Assets > Create > Stat > Unit and assign one per unit prefab.
    /// </summary>
    [CreateAssetMenu(fileName = "UnitStat", menuName = "Stat/Unit")]
    public class BaseUnitStat : ScriptableObject
    {
        public string Id;
        public float Health;
        public float Damage;
        public float Defense;
        public float Speed;

        [Tooltip("Radius, in world units, within which this unit can attack, heal, or interact.")]
        public float AttackRange;

        [Tooltip("Delay, in seconds, from the start of an attack cycle before damage/healing is applied.")]
        public float AttackDelay;

        [Tooltip("Time, in seconds, between the start of one attack cycle and the next.")]
        public float AttackInterval;
    }
}
