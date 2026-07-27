using General.Units;
using TMPro;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// Central game state: gold economy, win/lose condition, and the contact
    /// filters units use to distinguish friend from foe.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public bool GameOver;
        public UnitTower PlayerTower;
        public UnitTower EnemyTower;
        public int PlayerGold;
        public TextMeshProUGUI PlayerGoldText;
        public ContactFilter2D EnemyUnitFilter2D;
        public ContactFilter2D PlayerUnitFilter2D;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Update()
        {
            PlayerGoldText.text = PlayerGold.ToString();

            if (PlayerTower.IsHealthEmpty() || EnemyTower.IsHealthEmpty())
                GameOver = true;
        }
    }
}
