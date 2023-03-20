using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Kraft.ConnectionManagement
{
    internal class StartingHostState : OnlineState
    {
        ConnectionMethod m_ConnectionMethod;

        public StartingHostState Configure(ConnectionMethod connectionMethod)
        {
            m_ConnectionMethod = connectionMethod;
            return this;
        }

        public override void Enter()
        {
            _ = StartHost();
        }

        public override void Exit() { }

        public override void OnClientDisconnected(ulong clientId)
        {
            if (clientId == m_ConnectionManager.NetworkManager.LocalClientId)
            {
                StartHostFailed();
            }
        }

        private void StartHostFailed()
        {
            m_ConnectStatusPublisher.Publish(ConnectStatus.StartHostFailed);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
        }

        public override void OnServerStarted()
        {
            m_ConnectStatusPublisher.Publish(ConnectStatus.Success);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Hosting);
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var payloadBytes = request.Payload;
            var clientId = request.ClientNetworkId;
            // This happens when starting as a host, before the end of the StartHost call. In that case, we simply approve ourselves.
            if (clientId != m_ConnectionManager.NetworkManager.LocalClientId) return;

            var payload = System.Text.Encoding.UTF8.GetString(payloadBytes);
            var payloadData = JsonUtility.FromJson<ConnectionPayload>(payload);

            SessionManager.Instance.SetupConnectingPlayerSessionData(clientId, payloadData.playerId, payloadData.playerName);

            response.Approved = true;
            response.CreatePlayerObject = true;
        }

        async Task StartHost()
        {
            try
            {
                await m_ConnectionMethod.SetupHostConnectionAsync();

                if (!m_ConnectionManager.NetworkManager.StartHost())
                {
                    OnClientDisconnected(m_ConnectionManager.NetworkManager.LocalClientId);
                }
            }
            catch (Exception e)
            {
                StartHostFailed();
                throw e;
            }
        }
    }
}
