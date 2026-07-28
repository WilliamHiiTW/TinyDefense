using General.Units;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    /// <summary>
    /// Central game state: gold economy, start/win/lose flow, and the contact
    /// filters units use to distinguish friend from foe.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [Header("Gameplay State")]
        public bool GameOver;
        public UnitTower PlayerTower;
        public UnitTower EnemyTower;
        public int PlayerGold;
        public TextMeshProUGUI PlayerGoldText;
        public ContactFilter2D EnemyUnitFilter2D;
        public ContactFilter2D PlayerUnitFilter2D;

        [Header("Level Bounds")]
        [Tooltip("Leftmost world-space X position units are allowed to reach.")]
        public float LevelMinX = -10f;
        [Tooltip("Rightmost world-space X position units are allowed to reach.")]
        public float LevelMaxX = 10f;

        [Header("Pawn Cap")]
        [Tooltip("Maximum number of player Pawns allowed to be active at once, to prevent spamming free/cheap gold collectors.")]
        public int MaxActivePawns = 2;
        [Tooltip("Tracked automatically — incremented when a player Pawn is activated, decremented when one dies.")]
        public int ActivePlayerPawnCount { get; set; }
        public int ActiveEnemyPawnCount { get; set; }
        public TextMeshProUGUI PawnLimitText;

        [Header("Time Limit")]
        [Tooltip("If neither tower is destroyed within this many seconds, the match ends in a draw.")]
        public float TimeLimit = 120f;
        [Tooltip("Optional. Shown as the countdown, e.g. formatted as MM:SS. Leave unassigned if you don't want a timer displayed.")]
        public TextMeshProUGUI TimeRemainingText;

        [Header("UI Panels")]
        [Tooltip("Shown before the game starts. Hook your Start button's OnClick to StartGame().")]
        public GameObject StartPanel;
        [Tooltip("Shown when the player destroys the enemy tower.")]
        public GameObject WinPanel;
        [Tooltip("Shown when the enemy destroys the player tower. Hook your Restart button's OnClick to RestartGame().")]
        public GameObject LosePanel;
        [Tooltip("Shown when the time limit is reached with both towers still standing. Hook your Restart button's OnClick to RestartGame().")]
        public GameObject DrawPanel;

        private bool _gameStarted;
        private float _elapsedTime;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Start()
        {
            Time.timeScale = 0f;
            SetPanel(StartPanel, true);
            SetPanel(WinPanel, false);
            SetPanel(LosePanel, false);
            SetPanel(DrawPanel, false);
        }

        private void Update()
        {
            if (!_gameStarted || GameOver)
                return;

            PlayerGoldText.text = PlayerGold.ToString();
            CheckWinLoseCondition();
            PawnLimitText.text = $"{ActivePlayerPawnCount}/{MaxActivePawns}";
            
            if (GameOver)
                return;

            UpdateTimeLimit();
        }

        /// <summary>Hook this up to the Start button's OnClick.</summary>
        public void StartGame()
        {
            _gameStarted = true;
            Time.timeScale = 1f;
            SetPanel(StartPanel, false);
        }

        /// <summary>Hook this up to the Restart button's OnClick.</summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void CheckWinLoseCondition()
        {
            if (EnemyTower.IsHealthEmpty())
                EndGame(WinPanel);
            else if (PlayerTower.IsHealthEmpty())
                EndGame(LosePanel);
        }

        private void UpdateTimeLimit()
        {
            _elapsedTime += Time.deltaTime;
            float timeRemaining = Mathf.Max(0f, TimeLimit - _elapsedTime);

            if (TimeRemainingText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                TimeRemainingText.text = $"{minutes:00}:{seconds:00}";
            }

            if (_elapsedTime >= TimeLimit)
                EndGame(DrawPanel);
        }

        private void EndGame(GameObject panelToShow)
        {
            GameOver = true;
            Time.timeScale = 0f;
            SetPanel(panelToShow, true);
        }

        public void CloseGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void SetPanel(GameObject panel, bool isActive)
        {
            if (panel != null)
                panel.SetActive(isActive);
        }
    }
}