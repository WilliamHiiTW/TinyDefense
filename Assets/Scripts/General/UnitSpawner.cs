using General.Units;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace General
{
    /// <summary>
    /// A clickable structure that spawns a unit after a gold cost is paid and a
    /// spawn-duration timer (shown via <see cref="ProgressBar"/>) elapses.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class UnitSpawner : MonoBehaviour
    {
        public int SpawnCost;
        public GameObject SpawnPrefab;
        public Transform SpawnPoint;
        public float SpawnDuration;
        public Slider ProgressBar;

        private float _spawnTimer;
        private bool _isSpawning;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!_isSpawning)
                return;

            ProgressBar.value = _spawnTimer / SpawnDuration;
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < SpawnDuration)
                return;

            SpawnUnit();
            _spawnTimer = 0f;
            ProgressBar.value = 0f;
            _isSpawning = false;
        }

        private void OnMouseDown()
        {
            if (_isSpawning)
                return;

            if (_spriteRenderer != null)
                _spriteRenderer.sortingOrder += 100;

            if (IsPawnSpawner() && GameManager.instance.ActivePlayerPawnCount >= GameManager.instance.MaxActivePawns)
            {
                Debug.Log("[UnitSpawner] Spawn failed: pawn limit reached");
                return;
            }

            if (GameManager.instance.PlayerGold < SpawnCost)
            {
                Debug.Log("[UnitSpawner] Spawn failed: insufficient gold");
                return;
            }

            GameManager.instance.PlayerGold -= SpawnCost;
            _isSpawning = true;
        }

        /// <summary>True if this spawner produces Pawns specifically, so the pawn cap applies to it.</summary>
        private bool IsPawnSpawner()
        {
            return SpawnPrefab != null && SpawnPrefab.GetComponent<UnitPawn>() != null;
        }

        private void SpawnUnit()
        {
            Instantiate(SpawnPrefab, SpawnPoint.position, SpawnPoint.rotation);
        }
    }
}