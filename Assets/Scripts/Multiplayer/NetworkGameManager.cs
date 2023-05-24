using System;
using System.Collections.Generic;
using Managers;
using Mirror;
using UnityEngine;

namespace Multiplayer
{
    public class NetworkGameManager : NetworkManager
    {
        public static NetworkGameManager Instance;
        
        public GameState state;
        
        public static event Action<GameState> OnGameStateChanged;

        
        
        public void Awake()
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
        
        public static void SendMapToClient(NetworkConnection conn, List<GridCell> map)
        {
            if (map.Count == 0)
            {
                map = GridManager.Instance.gridCells;
                if (map.Count == 0)
                {
                    Debug.LogError("Map is empty");
                    return;
                }
            }
            
            string JsonMapData = GridManager.MapToJson(map);
            MapDataMessage mapDataMessage = new MapDataMessage {
                JsonMapData = JsonMapData
            };
            //Send
            conn.Send(mapDataMessage);
        }
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.LogError("Bruh");
            SendMapToClient(conn, GridManager.Instance.gridCells);
           
        }
        
        public void LockRoom()
        {
            this.maxConnections = 1;
        }
        
        public void UnlockRoom()
        {
            this.maxConnections = 2;
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
