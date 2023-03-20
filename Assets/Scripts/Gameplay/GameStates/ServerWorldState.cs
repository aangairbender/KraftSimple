using Kraft.ConnectionManagement;
using Kraft.Gameplay.Characters;
using Kraft.Utils;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Kraft.Gameplay
{
    [RequireComponent(typeof(NetcodeHooks))]
    public class ServerWorldState : GameStateBehaviour
    {
        [Tooltip("Make sure this is included in the NetworkManager's list of prefabs!")]
        [SerializeField] NetworkObject m_PlayerCharacterPrefab;
        [SerializeField] Transform m_SpawnPoint;
        [SerializeField] NetcodeHooks m_NetcodeHooks;

        // [Inject] ConnectionManager m_ConnectionManager;

        protected override void Awake()
        {
            base.Awake();
            m_NetcodeHooks.OnNetworkSpawnHook += M_NetcodeHooks_OnNetworkSpawnHook;
            m_NetcodeHooks.OnNetworkDespawnHook += M_NetcodeHooks_OnNetworkDespawnHook;
        }

        private void M_NetcodeHooks_OnNetworkDespawnHook()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }

        private void M_NetcodeHooks_OnNetworkSpawnHook()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }

            if (NetworkManager.Singleton.IsHost)
            {
                SpawnPlayer(NetworkManager.Singleton.LocalClientId);
            }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

            SessionManager.Instance.StartSession();
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            SpawnPlayer(clientId);
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
        }

        protected override void OnDestroy()
        {
            if (m_NetcodeHooks)
            {
                m_NetcodeHooks.OnNetworkSpawnHook -= M_NetcodeHooks_OnNetworkSpawnHook;
                m_NetcodeHooks.OnNetworkDespawnHook -= M_NetcodeHooks_OnNetworkDespawnHook;
            }
            base.OnDestroy();
        }

        private void SpawnPlayer(ulong clientId)
        {
            var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            var newPlayer = Instantiate(m_PlayerCharacterPrefab, Vector3.zero, Quaternion.identity);
            var newPlayerCharacter = newPlayer.GetComponent<ServerCharacter>();
            var physicsTransform = newPlayerCharacter.PhysicsWrapper.Transform;
            physicsTransform.SetPositionAndRotation(m_SpawnPoint.position, m_SpawnPoint.rotation);

            newPlayer.SpawnWithOwnership(clientId, true);
        }
    }
}
