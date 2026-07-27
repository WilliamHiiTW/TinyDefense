using General.Battle;
using UnityEngine;
using UnityEngine.Pool;

namespace Controller
{
    /// <summary>
    /// Manages a pooled set of <see cref="Projectile"/> instances for a ranged unit,
    /// avoiding repeated Instantiate/Destroy calls during combat.
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        public Projectile ProjectilePrefab;
        public float ProjectileAmount;

        private ObjectPool<Projectile> _projectiles;

        private void Awake()
        {
            _projectiles = new ObjectPool<Projectile>(
                CreateProjectile,
                OnGetProjectile,
                OnReleaseProjectile,
                OnDestroyProjectile
            );
        }

        /// <summary>Pre-warms the pool with <see cref="ProjectileAmount"/> inactive projectiles.</summary>
        public void Initialize()
        {
            if (ProjectilePrefab == null)
            {
                Debug.LogError("[ProjectileController] Projectile Prefab is null");
                return;
            }

            for (int i = 0; i < ProjectileAmount; i++)
            {
                Projectile spawned = CreateProjectile();
                spawned.Despawn += Despawn;
                Despawn(spawned);
            }
        }

        private Projectile CreateProjectile()
        {
            GameObject projectile = Instantiate(ProjectilePrefab.gameObject);
            return projectile.GetComponent<Projectile>();
        }

        private void OnGetProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
        }

        private void OnReleaseProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyProjectile(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        public Projectile Spawn()
        {
            return _projectiles.Get();
        }

        public void Despawn(Projectile projectile)
        {
            _projectiles.Release(projectile);
        }
    }
}
