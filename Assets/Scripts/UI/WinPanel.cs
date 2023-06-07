using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WinPanel : MonoBehaviour
    {
        [SerializeField]
        private Button okButton;
        
        private UIController uiController;

        private void Awake()
        {
            uiController = FindObjectOfType<UIController>();
        }

        private void OnEnable()
        {
            okButton.onClick.AddListener(OnOkButtonClick);
        }

        private void OnDisable()
        {
            okButton.onClick.RemoveAllListeners();
        }

        private void OnOkButtonClick()
        {
            uiController.IsWinPanelVisible(false);
        }
    }
}
