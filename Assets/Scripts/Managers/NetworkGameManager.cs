using System;
using System.Collections.Generic;
using Mirror;
using Multiplayer;
using UnityEngine;

namespace Managers
{
    public class NetworkGameManager : NetworkManager
    {
        public static NetworkGameManager Instance;
        
        public GameState state;
        
        public static event Action<GameState> OnGameStateChanged;

        
        public new void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            
            GridManager.GridGenerated += OnGridGenerated;
            
            
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
            
            // пределение размера пакета (например, 32768 байт)
            int packetSize = 32767;
            int numPackets = Mathf.CeilToInt((float)JsonMapData.Length / packetSize);

            Debug.Log(numPackets + "sended");
            
            
            // Разделение сообщения на пакеты и отправка их по одному
            for (int i = 0; i < numPackets; i++)
            {
                string packetData = JsonMapData.Substring(i * packetSize, Mathf.Min(packetSize, JsonMapData.Length - i * packetSize));

                MapDataMessage packet = new MapDataMessage
                {
                    PacketIndex = i,
                    NumPackets = numPackets,
                    PacketData = packetData
                };

                conn.Send(packet);
            }

            Debug.Log("Sent map to client");
        }
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.LogError("Bruh"); 
            
            if (conn.identity.isLocalPlayer)
            {
                Debug.Log("Host joined");
                GridManager.Instance.GenerateGridFromJson();
            }
            else
            {
                Debug.Log("Client joined");
                SendMapToClient(conn, GridManager.Instance.gridCells);
            }
            

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
                case GameState.GeneratingMap:
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
        GeneratingMap,
        WaitingForPlayers,
        SettingUp,
        Player1Turn,
        Player1TurnEnd,
        Player2Turn,
        Player2TurnEnd,
        GameOver
    }
}
