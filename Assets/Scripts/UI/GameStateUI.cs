using Managers;
using Multiplayer;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameStateUI : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private NetworkGameManager _gameManager;
        
        void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _gameManager = NetworkGameManager.Instance;
        }

        
        void Update()
        {
            _text.text = _gameManager.state.ToString();
        }
    }
}
