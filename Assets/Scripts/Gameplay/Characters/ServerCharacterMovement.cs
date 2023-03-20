using Unity.Netcode;
using UnityEngine;

namespace Kraft.Gameplay.Characters
{
    public class ServerCharacterMovement : NetworkBehaviour
    {
        [SerializeField] Rigidbody m_Rigidbody;
        [SerializeField] ServerCharacter m_ServerCharacter;

        Vector3 m_DesiredMoveDirection = Vector3.zero;
        bool m_IsSprinting = false;

        private void Awake()
        {
            enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            enabled = true;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;

            enabled = false;
        }

        [ServerRpc]
        public void SetDesiredMoveDirectionServerRpc(Vector3 desiredMoveDirection)
        {
            m_DesiredMoveDirection = desiredMoveDirection;
        }

        [ServerRpc]
        public void SetSprintingServerRpc(bool isSprinting)
        {
            m_IsSprinting = isSprinting;
        }

        private void FixedUpdate()
        {
            PerformMovement();
        }

        private void PerformMovement()
        {
            var movementSpeed = m_IsSprinting ? 4.0f : 1.5f;
            var movementVector = m_DesiredMoveDirection * movementSpeed * Time.fixedDeltaTime;
            m_ServerCharacter.AnimationHandler.SetSpeed(m_DesiredMoveDirection.magnitude * movementSpeed);

            if (movementVector.magnitude < 0.001f) return;

            transform.Translate(movementVector, Space.World);

            // transform.rotation = Quaternion.LookRotation(movementVector);
            m_ServerCharacter.ClientCharacter.ModelTransform.transform.rotation =
                Quaternion.Lerp(m_ServerCharacter.ClientCharacter.ModelTransform.transform.rotation, Quaternion.LookRotation(movementVector, Vector3.up), Time.deltaTime * 15.0f);

            m_Rigidbody.position = transform.position;
            // m_Rigidbody.rotation = transform.rotation;
        }
    }
}
