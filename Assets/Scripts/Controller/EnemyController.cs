using System.Collections.Generic;
using General;
using UnityEngine;

namespace Controller
{
    /// <summary>
    /// Periodically spawns enemy units at a fixed spawn point.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [Tooltip("Prefab spawned at each interval. Must have a Unit component.")]
        public GameObject ArcherPrefab;
        public List<Transform> SpawnPoints;
        public float SpawnInterval;

        private float _spawnTimer;

        private void Update()
        {
            if (ArcherPrefab == null || SpawnPoints.Count == 0)
                return;

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < SpawnInterval)
                return;

            _spawnTimer = 0f;
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            Transform spawnPoint = SpawnPoints[0];
            GameObject enemy = Instantiate(ArcherPrefab, spawnPoint.position, spawnPoint.rotation);

            if (!enemy.TryGetComponent(out Unit unit))
            {
                Debug.LogWarning($"[EnemyController] Spawned prefab '{ArcherPrefab.name}' has no Unit component.");
                return;
            }

            unit.IsEnemyUnit = true;
            unit.Initialize();
            unit.Activate();
        }
    }
}
