using Mirror;

namespace Multiplayer
{
    public struct MapDataMessage : NetworkMessage
    {
        public string JsonMapData;
    }
}