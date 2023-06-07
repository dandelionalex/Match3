using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private ConfigPanel configPanel;

        [SerializeField]
        private GamePanel gamePanel;

        [SerializeField]
        private WinPanel winPanel;

        public void IsConfigPanelVisible(bool value)
        {
            configPanel.gameObject.SetActive(value);
        }

        public void IsGamePanelVisible(bool value)
        {
            gamePanel.gameObject.SetActive(value);
        }

        public void IsWinPanelVisible(bool value)
        {
            winPanel.gameObject.SetActive(value);
        }
    }
}