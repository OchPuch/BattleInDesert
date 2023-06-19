using System;
using Managers;
using Mirror;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerController : NetworkBehaviour
    {
        
        private string _receivedMapData;
        private int _numPacketsReceived;

        private void Awake()
        {
            NetworkClient.RegisterHandler<MapDataMessage>(OnMapDataReceived);
        }

        public void OnMapDataReceived(MapDataMessage message)
        {
            //if player is not host
            Debug.Log ("Received map data");
            
            // Добавляем полученные данные к собранному сообщению
            if (_receivedMapData == null)
            {
                _receivedMapData = message.PacketData;
                _numPacketsReceived = 1;
            }
            else
            {
                _receivedMapData += message.PacketData;
                _numPacketsReceived++;
            }

            // Проверяем, является ли текущий пакет последним
            if (_numPacketsReceived == message.NumPackets)
            {
                Debug.Log("Received full map data");
                Debug.Log(_numPacketsReceived + "received");
                // Полное сообщение было собрано, вызываем метод для обработки
                GridManager.Instance.GenerateGridFromJson(_receivedMapData);
                _receivedMapData = null;
                _numPacketsReceived = 0;
            }
            
            
        }
    }
}