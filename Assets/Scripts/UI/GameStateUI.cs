using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameStateUI : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private GameManager _gameManager;
        
        void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _gameManager = GameManager.Instance;
        }

        
        void Update()
        {
            _text.text = _gameManager.state.ToString();
        }
    }
}
