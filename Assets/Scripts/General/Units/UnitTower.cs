namespace General.Units
{
    /// <summary>The player's or enemy's base structure. Loses when its health reaches zero.</summary>
    public class UnitTower : Unit
    {
        public override void Initialize()
        {
            _currentHealth = BaseUnitStat.Health;
        }

        private void Update()
        {
            base.OnUpdate();
        }
    }
}
