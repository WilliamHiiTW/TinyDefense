using System.Collections.Generic;
using General;
using General.Waves;
using Manager;
using UnityEngine;

namespace Controller
{
    /// <summary>
    /// Spawns enemy units on a scripted timeline defined by a <see cref="WaveConfig"/>,
    /// rather than a single repeating prefab/interval.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [Tooltip("The scripted spawn timeline for this wave/demo.")]
        public WaveConfig Wave;

        [Tooltip("Spawn points, indexed by each wave entry's SpawnPointIndex.")]
        public List<Transform> SpawnPoints;

        public GameObject PawnPrefab;

        private float _waveTimer;
        private float _replacementPawnSpawnTimer;
        private int _nextEntryIndex;
        private bool _firstPawnSpawned;

        private void OnEnable()
        {
            _waveTimer = 0f;
            _nextEntryIndex = 0;
            _replacementPawnSpawnTimer = 0f;
            _firstPawnSpawned = false;
        }

        private void Update()
        {
            if(!_firstPawnSpawned) _firstPawnSpawned = GameManager.instance.ActiveEnemyPawnCount > 0;
            if (Wave == null || Wave.Entries.Count == 0)
                return;

            if (_firstPawnSpawned && GameManager.instance.ActiveEnemyPawnCount < 2)
            {
                _replacementPawnSpawnTimer += Time.deltaTime;
                if (_replacementPawnSpawnTimer >= 1.5f)
                {
                    _replacementPawnSpawnTimer = 0f;
                    SpawnReplacementPawn();
                }

                if(GameManager.instance.ActiveEnemyPawnCount <= 0) return;
            }
            
            _waveTimer += Time.deltaTime;

            // Spawn every entry whose time has come; a while-loop (not "if") handles
            // multiple entries landing in the same frame (e.g. two enemies at t=0).
            while (_nextEntryIndex < Wave.Entries.Count && _waveTimer >= Wave.Entries[_nextEntryIndex].SpawnTime)
            {
                SpawnEnemy(Wave.Entries[_nextEntryIndex]);
                _nextEntryIndex++;
            }
        }

        private void SpawnReplacementPawn()
        {
            Transform spawnPoint = SpawnPoints[0];
            GameObject enemy = Instantiate(PawnPrefab, spawnPoint.position, spawnPoint.rotation);

            if (!enemy.TryGetComponent(out Unit unit))
            {
                Debug.LogWarning($"[EnemyController] Spawned prefab '{PawnPrefab.name}' has no Unit component.");
                return;
            }

            unit.IsEnemyUnit = true;
            unit.Initialize();
            unit.Activate();
        }

        private void SpawnEnemy(WaveEntry entry)
        {
            if (entry.EnemyPrefab == null)
            {
                Debug.LogWarning("[EnemyController] Wave entry has no EnemyPrefab assigned.");
                return;
            }

            if (entry.SpawnPointIndex < 0 || entry.SpawnPointIndex >= SpawnPoints.Count)
            {
                Debug.LogWarning($"[EnemyController] Wave entry spawn point index {entry.SpawnPointIndex} is out of range.");
                return;
            }

            Transform spawnPoint = SpawnPoints[entry.SpawnPointIndex];
            GameObject enemy = Instantiate(entry.EnemyPrefab, spawnPoint.position, spawnPoint.rotation);

            if (!enemy.TryGetComponent(out Unit unit))
            {
                Debug.LogWarning($"[EnemyController] Spawned prefab '{entry.EnemyPrefab.name}' has no Unit component.");
                return;
            }

            unit.IsEnemyUnit = true;
            unit.Initialize();
            unit.Activate();
        }
    }
}
