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
        private TMP_InputField leftToWin;

        [SerializeField]
        private TMP_InputField variaty;

        [SerializeField]
        private Button startButton;

        private BoardPresenter boardPresenter;
        
        private GameSession gameSession;

        private void Awake()
        {
            gameSession = FindObjectOfType<GameSession>();
            boardPresenter = FindObjectOfType<BoardPresenter>();
        }

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
            int w = int.Parse(width.text);
            int h = int.Parse(height.text);
            int leftTW = int.Parse(leftToWin.text);
            int v = Mathf.Clamp( int.Parse(variaty.text), 1, 5 );
            Debug.Log($"v: {v}");
            gameSession.StartGame( w,h, leftTW, v );
        }
    }
}