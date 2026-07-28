using System;
using System.Collections.Generic;
using UnityEngine;

namespace General.Waves
{
    /// <summary>A single scripted spawn: which enemy, when, and from where.</summary>
    [Serializable]
    public class WaveEntry
    {
        [Tooltip("Enemy prefab to spawn. Must have a Unit component.")]
        public GameObject EnemyPrefab;

        [Tooltip("Time, in seconds after the wave starts, at which this enemy spawns.")]
        public float SpawnTime;

        [Tooltip("Index into the spawner's SpawnPoints list.")]
        public int SpawnPointIndex;
    }

    /// <summary>
    /// Designer-configurable timeline of enemy spawns for a level/demo. Create via
    /// Assets > Create > Config > Wave and assign to an <see cref="Controller.EnemyController"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Config/Wave")]
    public class WaveConfig : ScriptableObject
    {
        [Tooltip("Enemies to spawn, in the order and timing defined here. Automatically kept sorted by SpawnTime.")]
        public List<WaveEntry> Entries = new List<WaveEntry>();
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            Entries.Sort((a, b) => a.SpawnTime.CompareTo(b.SpawnTime));
        }
#endif
    }
}