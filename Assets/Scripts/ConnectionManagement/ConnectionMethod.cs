using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Kraft.ConnectionManagement
{
    [Serializable]
    public class ConnectionPayload
    {
        public string playerId;
        public string playerName;
        public bool isDebug;
    }

    public abstract class ConnectionMethod
    {
        protected ConnectionManager m_ConnectionManager;
        protected readonly string m_PlayerName;

        public abstract Task SetupHostConnectionAsync();
        public abstract Task SetupClientConnectionAsync();

        public ConnectionMethod(ConnectionManager connectionManager, string playerName)
        {
            m_ConnectionManager = connectionManager;
            m_PlayerName = playerName;
        }

        protected void SetConnectionPayload(string playerId, string playerName)
        {
            var payload = JsonUtility.ToJson(new ConnectionPayload
            {
                playerId = playerId,
                playerName = playerName,
                isDebug = Debug.isDebugBuild,
            });

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            m_ConnectionManager.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
        }

        protected string GetPlayerId()
        {
            return ClientPrefs.GetGuid();
        }
    }

    public sealed class IPConnectionMethod : ConnectionMethod
    {
        private readonly string m_IPAddress;
        private readonly ushort m_Port;

        public IPConnectionMethod(string ip, ushort port, ConnectionManager connectionManager, string playerName)
            : base(connectionManager, playerName)
        {
            m_IPAddress = ip;
            m_Port = port;
        }

        public override Task SetupClientConnectionAsync()
        {
            SetupCommonConnection();
            return Task.CompletedTask;
        }

        public override Task SetupHostConnectionAsync()
        {
            SetupCommonConnection();
            return Task.CompletedTask;
        }

        private void SetupCommonConnection()
        {
            SetConnectionPayload(GetPlayerId(), m_PlayerName);
            var utp = m_ConnectionManager.NetworkManager.NetworkConfig.NetworkTransport as UnityTransport;
            if (utp != null)
            {
                utp.SetConnectionData(m_IPAddress, m_Port);
            }
            else
            {
                Debug.LogError("Configured NetworkTransport is not UnityTransport");
            }
        }
    }
}
