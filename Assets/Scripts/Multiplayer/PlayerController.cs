using Managers;
using Mirror;

namespace Multiplayer
{
    public class PlayerController : NetworkBehaviour
    {
        public void OnMapDataReceived(MapDataMessage mapDataMessage)
        {
            //if player is not host
            if (!isServer)
            {
                GridManager.Instance.GenerateGridFromJson(mapDataMessage.JsonMapData);
            }
        }
    }
}