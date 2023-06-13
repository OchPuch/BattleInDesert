using Mirror;

namespace Multiplayer
{
    public struct MapDataMessage : NetworkMessage
    {
        public int PacketIndex;
        public int NumPackets;
        public string PacketData;
    }
}