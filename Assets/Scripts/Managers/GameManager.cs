using System;
using Mirror;
using UnityEngine;

namespace Managers
{
    public class GameManager : NetworkBehaviour
    {
        
        public GameManager Instance;

        public GameState state;
    
        public static event Action<GameState> OnGameStateChanged;

        
        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            GridManager.GridGenerated += OnGridGenerated;
            UpdateState(GameState.ChoosingMap);
        }

        private void OnGridGenerated()
        {
            Debug.Log("Grid Generated");
            UpdateState(GameState.WaitingForPlayers);

        }
        
        public void UpdateState(GameState newState)
        {
            state = newState;
            switch (newState)
            {
                case GameState.ChoosingMap:
                    break;
                case GameState.WaitingForPlayers:
                    break;
                case GameState.SettingUp:
                    break;
                case GameState.Player1Turn:
                    break;
                case GameState.Player1TurnEnd:
                    break;
                case GameState.Player2Turn:
                    break;
                case GameState.Player2TurnEnd:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        
            OnGameStateChanged?.Invoke(newState);
        
        }
    }


    public enum GameState
    {
        ChoosingMap,
        WaitingForPlayers,
        SettingUp,
        Player1Turn,
        Player1TurnEnd,
        Player2Turn,
        Player2TurnEnd,
        GameOver
    }
}