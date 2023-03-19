using Kraft.Utils;
using Unity.Netcode;
using VContainer;

namespace Kraft.ConnectionManagement
{
    public enum ConnectStatus
    {
        Undefined,
        Success,                  // client successfully connected. This may also be a successful reconnect.
        ServerFull,               // can't join, server is already at capacity.
        LoggedInAgain,            // logged in on a separate client, causing this one to be kicked out.
        UserRequestedDisconnect,  // Intentional Disconnect triggered by the user.
        GenericDisconnect,        // server disconnected, but no specific reason given.
        Reconnecting,             // client lost connection and is attempting to reconnect.
        IncompatibleBuildType,    // client build type is incompatible with server.
        HostEndedSession,         // host intentionally ended the session.
        StartHostFailed,          // server failed to bind
        StartClientFailed         // failed to connect to server and/or invalid network endpoint
    }

    public abstract class ConnectionState
    {
        [Inject]
        protected ConnectionManager m_ConnectionManager;

        [Inject]
        protected IPublisher<ConnectStatus> m_ConnectStatusPublisher;

        public abstract void Enter();
        public abstract void Exit();

        public virtual void OnClientConnected(ulong clientId) { }
        public virtual void OnClientDisconnected(ulong clientId) { }
        
        public virtual void OnServerStarted() { }

        public virtual void StartClientIP(string playerName, string ipaddress, int port) { }
        public virtual void StartHostIP(string playerName, string ipaddress, int port) { }

        public virtual void OnUserRequestedShutdown() { }

        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }

        public virtual void OnTransportFailure() { }
    }
}
