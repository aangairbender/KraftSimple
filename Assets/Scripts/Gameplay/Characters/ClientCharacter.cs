using Kraft.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Kraft.Gameplay.Characters
{
    /// <summary>
    /// Is responsible for displaying a character on the client's screen based on state information sent by the server.
    /// </summary>
    public class ClientCharacter : NetworkBehaviour
    {
        [SerializeField] Animator m_ClientVisualsAnimator;
        [SerializeField] Transform m_CameraFollow;
        [SerializeField] Transform m_ModelTransform;

        ServerCharacter m_ServerCharacter;
        PhysicsWrapper m_PhysicsWrapper;

        public Animator Animator => m_ClientVisualsAnimator;
        public Transform CameraFollow => m_CameraFollow;

        public Transform ModelTransform => m_ModelTransform;

        private void Awake()
        {
            enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient || transform.parent == null)
            {
                return;
            }

            enabled = true;
            m_ServerCharacter = GetComponentInParent<ServerCharacter>();
            m_PhysicsWrapper = m_ServerCharacter.PhysicsWrapper;

            // sync our visualization position & rotation to the most up to date version received from server
            transform.SetPositionAndRotation(m_PhysicsWrapper.Transform.position, m_PhysicsWrapper.Transform.rotation);

            if (m_ServerCharacter.IsOwner)
            {
                var cameraController = gameObject.AddComponent<CameraController>();
                cameraController.AttachVirtualCamera(m_CameraFollow);
            }
        }

        public override void OnNetworkDespawn()
        {

        }
    }
}
