using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Kraft.ConnectionManagement
{
    public struct SessionPlayerData
    {
        public bool IsConnected { get; set; }
        public ulong ClientId { get; set; }
        public string PlayerName { get; set; }
    }

    /// <summary>
    /// This class uses a unique player ID to bind a player to a session. Once that player connects to a host, the host
    /// associates the current ClientID to the player's unique ID. If the player disconnects and reconnects to the same
    /// host, the session is preserved.
    /// </summary>
    /// <remarks>
    /// Using a client-generated player ID and sending it directly could be problematic, as a malicious user could
    /// intercept it and reuse it to impersonate the original user. We are currently investigating this to offer a
    /// solution that handles security better.
    /// </remarks>
    public class SessionManager
    {
        static SessionManager s_Instance;
        public static SessionManager Instance => s_Instance ??= new SessionManager();

        readonly Dictionary<ulong, string> m_ClientIdToPlayerId = new Dictionary<ulong, string>();
        readonly Dictionary<string, SessionPlayerData> m_PlayerData = new Dictionary<string, SessionPlayerData>();

        bool m_HasSessionStarted;

        public void DisconnectClient(ulong clientId)
        {
            if (m_HasSessionStarted)
            {
                if (!m_ClientIdToPlayerId.TryGetValue(clientId, out var playerId)) return;
                if (!m_PlayerData.TryGetValue(playerId, out var playerData) || playerData.ClientId != clientId) return;

                // Mark client as disconnected, but keep their data so they can reconnect.
                playerData.IsConnected = false;
                m_PlayerData[playerId] = playerData;
            }
            else
            {
                // Session has not started, no need to keep their data
                if (!m_ClientIdToPlayerId.TryGetValue(clientId, out var playerId)) return;
                m_ClientIdToPlayerId.Remove(clientId);

                if (!m_PlayerData.TryGetValue(playerId, out var playerData) || playerData.ClientId != clientId) return;
                m_PlayerData.Remove(playerId);
            }
        }

        public bool IsDuplicateConnection(string playerId)
        {
            return m_PlayerData.TryGetValue(playerId, out var playerData) && playerData.IsConnected;
        }

        public void SetupConnectingPlayerSessionData(ulong clientId, string playerId, string playerName)
        {
            if (IsDuplicateConnection(playerId))
            {
                Debug.LogError($"Player ID {playerId} already exists. This is a duplicate connection. Rejecting this session data.");
                return;
            }

            var isReconnecting = m_PlayerData.TryGetValue(playerId, out var playerData) && !playerData.IsConnected;

            if (isReconnecting)
            {
                Debug.Log($"Player ID {playerId} is reconnecting");
            }
            else
            {
                Debug.Log($"New player ID {playerId} session created");
            }

            playerData.ClientId = clientId;
            playerData.PlayerName = playerName;
            playerData.IsConnected = true;

            m_ClientIdToPlayerId[clientId] = playerId;
            m_PlayerData[playerId] = playerData;
        }

        public void StartSession()
        {
            m_HasSessionStarted = true;
        }

        public void EndSession()
        {
            m_HasSessionStarted = false;
        }

        public void EndServer()
        {
            m_PlayerData.Clear();
            m_ClientIdToPlayerId.Clear();
            m_HasSessionStarted = false;
        }

        public SessionPlayerData? GetPlayerData(string playerId)
        {
            if (m_PlayerData.TryGetValue(playerId, out var playerData))
            {
                return playerData;
            }

            Debug.Log($"No PlayerData of matching player ID found: {playerId}");
            return null;
        }

        public string GetPlayerId(ulong clientId)
        {
            if (m_ClientIdToPlayerId.TryGetValue(clientId, out var playerId))
            {
                return playerId;
            }

            Debug.Log($"No client player ID found mapped to the given client ID: {clientId}");
            return null;
        }

        public SessionPlayerData? GetPlayerData(ulong clientId)
        {
            var playerId = GetPlayerId(clientId);
            return playerId != null ? GetPlayerData(playerId) : null;
        }
    }
}
