using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class ConfigPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField width;
        
        [SerializeField]
        private TMP_InputField height;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private BoardPresenter boardPresenter;

        private void OnEnable()
        {
            startButton.onClick.AddListener(OnStartClick);
        }

        private void OnDisable() 
        {
            startButton.onClick.RemoveAllListeners();
        }

        private void OnStartClick()
        {
            Debug.Log($"width.text {width.text} height {height.text}");
            int w = int.Parse(width.text);
            int h = int.Parse(height.text);
            Debug.Log($"OnStartClick w: {w}, h: {h}");
            boardPresenter.GenerateBoard(w,h);
            gameObject.SetActive(false);
        }
    }
}