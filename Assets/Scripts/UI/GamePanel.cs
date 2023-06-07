using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class GamePanel : MonoBehaviour
    {
        private BoardPresenter boardPresenter;
        private GameSession gameSession;

        [SerializeField]
        private TMP_Text leftToWin;

        [SerializeField]
        private Button cheatButton;

        [SerializeField]
        private Button restartButton;

        private void Awake()
        {
            boardPresenter = FindObjectOfType<BoardPresenter>();
            gameSession = FindObjectOfType<GameSession>();
        }
        
        private void OnEnable() 
        {
            cheatButton.onClick.AddListener(OnCheatButtonClick);
            restartButton.onClick.AddListener(OnResratButtonClick);
            leftToWin.text = boardPresenter.LeftToWin.ToString();
            boardPresenter.matchCountUpdated += OnMathesUpdated;
        }

        private void OnDisable() 
        {
            cheatButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            boardPresenter.matchCountUpdated -= OnMathesUpdated;
        }

        private void OnCheatButtonClick()
        {
            gameSession.CheateModeEnabled();        
        }

        private void OnMathesUpdated(int value)
        {
            leftToWin.text = value.ToString();
        }

        private void OnResratButtonClick()
        {
            gameSession.FinishGame();
        }
    }
}

