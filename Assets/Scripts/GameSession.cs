using UnityEngine;
using UI;

public class GameSession : MonoBehaviour
{
    private UIController uiController;
    private BoardPresenter boardPresenter;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
        boardPresenter = FindObjectOfType<BoardPresenter>();
    }

    public void StartGame(int w, int h, int leftTW, int variaty)
    {
        boardPresenter.GenerateBoard(w, h, leftTW, variaty);
        uiController.IsGamePanelVisible(true);
        uiController.IsConfigPanelVisible(false);
    }

    public void FinishGame()
    {
        uiController.IsGamePanelVisible(false);
        uiController.IsWinPanelVisible(false);
        uiController.IsConfigPanelVisible(true);
        boardPresenter.Reset();
    }

    public void CheateModeEnabled()
    {
        boardPresenter.CheatModeEnabled();
    }
}
