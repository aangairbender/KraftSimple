using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Kraft.ConnectionManagement
{
    /// <summary>
    /// This state machine handles connection through the NetworkManager. It is responsible for listening to
    /// NetworkManger callbacks and other outside calls and redirecting them to the current ConnectionState object.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        private ConnectionState m_CurrentState;

        [Inject] NetworkManager m_NetworkManager;
        public NetworkManager NetworkManager => m_NetworkManager;

        [Inject]
        IObjectResolver m_Resolver;

        public int ReconnectAttempts = 2;
        public int MaxConnectedPlayers = 8;

        internal readonly OfflineState m_Offline = new OfflineState();
        internal readonly StartingHostState m_StartingHost = new StartingHostState();
        internal readonly HostingState m_Hosting = new HostingState();
        internal readonly ClientConnectingState m_ClientConnecting = new ClientConnectingState();
        internal readonly ClientReconnectingState m_ClientReconnecting = new ClientReconnectingState();
        internal readonly ClientConnectedState m_ClientConnected = new ClientConnectedState();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            var states = new List<ConnectionState>() {
                m_Offline, m_StartingHost, m_Hosting, m_ClientConnecting, m_ClientReconnecting, m_ClientConnected
            };
            foreach (var state in states)
            {
                m_Resolver.Inject(state);
            }

            m_CurrentState = m_Offline;

            NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
            NetworkManager.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.OnTransportFailure += NetworkManager_OnTransportFailure;
        }

        private void OnDestroy()
        {
            NetworkManager.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
            NetworkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
            NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
            NetworkManager.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
            NetworkManager.OnTransportFailure -= NetworkManager_OnTransportFailure;
        }

        internal void ChangeState(ConnectionState nextState)
        {
            Debug.Log($"{name}: Changed connection state from {m_CurrentState.GetType().Name} to {nextState.GetType().Name}.");

            m_CurrentState?.Exit();
            m_CurrentState = nextState;
            m_CurrentState.Enter();
        }

        private void NetworkManager_OnTransportFailure()
        {
            m_CurrentState?.OnTransportFailure();
        }

        private void NetworkManager_OnServerStarted()
        {
            m_CurrentState?.OnServerStarted();
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            m_CurrentState?.OnClientDisconnected(clientId);
        }

        private void NetworkManager_OnClientConnectedCallback(ulong clientId)
        {
            m_CurrentState?.OnClientConnected(clientId);
        }

        private void NetworkManager_ConnectionApprovalCallback(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            m_CurrentState?.ApprovalCheck(request, response);
        }

        public void StartClientIp(string playerName, string ipaddress, int port)
        {
            m_CurrentState.StartClientIP(playerName, ipaddress, port);
        }

        public void StartHostIp(string playerName, string ipaddress, int port)
        {
            m_CurrentState.StartHostIP(playerName, ipaddress, port);
        }

        public void RequestShutdown()
        {
            m_CurrentState.OnUserRequestedShutdown();
        }
    }
}
