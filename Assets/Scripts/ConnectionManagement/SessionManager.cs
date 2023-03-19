using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kraft.ConnectionManagement
{
    public interface ISessionPlayerData
    {
        bool IsConnected { get; set; }
        ulong ClientId { get; set; }
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
    /// <typeparam name="T"></typeparam>
    public class SessionManager<T> where T : struct, ISessionPlayerData
    {
        static SessionManager<T> s_Instance;
        public static SessionManager<T> Instance => s_Instance ??= new SessionManager<T>();

        readonly Dictionary<ulong, string> m_ClientIdToPlayerId = new Dictionary<ulong, string>();
        readonly Dictionary<string, T> m_PlayerData = new Dictionary<string, T>();

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
    }
}
