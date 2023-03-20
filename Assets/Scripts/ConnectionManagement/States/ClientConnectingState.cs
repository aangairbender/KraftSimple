using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Kraft.ConnectionManagement
{
    internal class ClientConnectingState : OnlineState
    {
        ConnectionMethod m_ConnectionMethod;

        public ClientConnectingState Configure(ConnectionMethod connectionMethod)
        {
            m_ConnectionMethod = connectionMethod;
            return this;
        }

        public override void Enter()
        {
            _ = ConnectClientAsync();
        }

        public override void Exit() { }

        public override void OnClientConnected(ulong clientId)
        {
            m_ConnectStatusPublisher.Publish(ConnectStatus.Success);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnected);
        }

        public override void OnClientDisconnected(ulong clientId)
        {
            // client ID is for sure ours here
            StartingClientFailed();
        }

        protected void StartingClientFailed()
        {
            var disconnectReason = m_ConnectionManager.NetworkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                m_ConnectStatusPublisher.Publish(ConnectStatus.StartClientFailed);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                m_ConnectStatusPublisher.Publish(connectStatus);
            }
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
        }

        internal async Task ConnectClientAsync()
        {
            try
            {
                // Setup NGO with current connection method
                await m_ConnectionMethod.SetupClientConnectionAsync();

                // NGO's StartClient launches everything
                if (!m_ConnectionManager.NetworkManager.StartClient())
                {
                    throw new Exception("NetworkManager StartClient failed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error connecting client, see following exception");
                Debug.LogException(e);
                StartingClientFailed();
                throw e;
            }
        }
    }
}
