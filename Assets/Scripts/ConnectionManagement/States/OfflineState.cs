using Kraft.SceneManagement;
using UnityEngine.SceneManagement;

namespace Kraft.ConnectionManagement
{
    public class OfflineState : ConnectionState
    {
        const string k_MainMenuSceneName = "MainMenu";

        public override void Enter()
        {
            m_ConnectionManager.NetworkManager.Shutdown();
            if (SceneManager.GetActiveScene().name != k_MainMenuSceneName)
            {
                SceneLoaderWrapper.Instance.LoadScene(k_MainMenuSceneName);
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
