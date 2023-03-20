using Kraft.SceneManagement;
using UnityEngine;

namespace Kraft.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to a connected client. When being disconnected, transitions to the
    /// ClientReconnecting state if no reason is given, or to the Offline state.
    /// </summary>
    internal class ClientConnectedState : OnlineState
    {
        public override void Enter()
        {
            // TODO: replace this with proper code (no scene management in connection management)
            SceneLoaderWrapper.Instance.LoadScene("World", useNetworkSceneManager: true);
        }

        public override void Exit() { }

        public override void OnClientDisconnected(ulong clientId)
        {
            var disconnectReason = m_ConnectionManager.NetworkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                m_ConnectStatusPublisher.Publish(ConnectStatus.Reconnecting);
                m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientReconnecting);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                m_ConnectStatusPublisher.Publish(connectStatus);
                m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
            }
        }
    }
}
