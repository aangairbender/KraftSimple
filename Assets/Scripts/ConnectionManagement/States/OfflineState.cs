using Kraft.SceneManagement;
using UnityEngine.SceneManagement;

namespace Kraft.ConnectionManagement
{
    internal class OfflineState : ConnectionState
    {
        public override void Enter()
        {
            m_ConnectionManager.NetworkManager.Shutdown();
            // TODO: replace this with proper code (no scene management in connection management)
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SceneLoaderWrapper.Instance.LoadScene("MainMenu", useNetworkSceneManager: false);
            }
        }

        public override void Exit() { }

        public override void StartClientIP(string playerName, string ipaddress, int port)
        {
            var connectionMethod = new IPConnectionMethod(ipaddress, (ushort)port, m_ConnectionManager, playerName);
            // confugure in case of future reconnections
            m_ConnectionManager.m_ClientReconnecting.Configure(connectionMethod);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnecting.Configure(connectionMethod));
        }

        public override void StartHostIP(string playerName, string ipaddress, int port)
        {
            var connectionMethod = new IPConnectionMethod(ipaddress, (ushort)port, m_ConnectionManager, playerName);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHost.Configure(connectionMethod));
        }
    }
}
