using System.Collections.Generic;
using Managers;
using Mirror;
using UnityEngine;

namespace Multiplayer
{
    public class NetworkGameManager : NetworkManager
    {
        public static NetworkGameManager Instance;
        
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
    }
}