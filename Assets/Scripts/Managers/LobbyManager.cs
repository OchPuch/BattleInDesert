using Mirror;

namespace Managers
{
    public class LobbyManager
    {
        public NetworkManager networkManager;
        public void LockRoom()
        {
            networkManager.maxConnections = 1;
        }
        
        public void UnlockRoom()
        {
            networkManager.maxConnections = 2;
        }
    }
}