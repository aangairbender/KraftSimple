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

        internal readonly OfflineState m_Offline = new OfflineState();
        internal readonly StartingHostState m_StartingHost = new StartingHostState();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
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
    }
}
