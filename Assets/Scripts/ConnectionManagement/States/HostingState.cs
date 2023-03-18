using Kraft.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Kraft.ConnectionManagement
{
    public class HostingState : ConnectionState
    {
        // used in ApprovalCheck. This is intended as a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
        const int k_MaxConnectPayload = 1024;
        const string k_WorldSceneName = "World";

        public override void Enter()
        {
            SceneLoaderWrapper.Instance.LoadScene(k_WorldSceneName);
        }

        public override void Exit()
        {
            // SessionManager<SessionPlayerData>.Instance.OnServerEnded();
        }

        public override void OnClientDisconnected(ulong clientId)
        {
            if (clientId == m_ConnectionManager.NetworkManager.LocalClientId)
            {
                m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
            }
            else
            {
                //var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
                //if (playerId != null)
                //{
                //    var sessionData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(playerId);
                //    if (sessionData.HasValue)
                //    {
                //        m_ConnectionEventPublisher.Publish(new ConnectionEventMessage() { ConnectStatus = ConnectStatus.GenericDisconnect, PlayerName = sessionData.Value.PlayerName });
                //    }
                //    SessionManager<SessionPlayerData>.Instance.DisconnectClient(clientId);
                //}
            }
        }
    }
}
